using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Configuration;
using MutfakMessageHub.Outbox;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Core;

/// <summary>
/// Default implementation of <see cref="IMessageHub"/>.
/// </summary>
public class MessageHub : IMessageHub
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MutfakMessageHubOptions _options;
    private readonly ILogger<MessageHub>? _logger;
    private static readonly ConcurrentDictionary<Type, MethodInfo> HandlerMethodCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageHub"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="options">The options.</param>
    /// <param name="logger">Optional logger.</param>
    public MessageHub(
        IServiceProvider serviceProvider,
        MutfakMessageHubOptions options,
        ILogger<MessageHub>? logger = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        _logger?.LogDebug("Sending request of type {RequestType}", requestType.Name);

        // Get handler
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.Name}");
        }

        // Get behaviors
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = _serviceProvider.GetServices(behaviorType).Where(b => b != null).Cast<object>().ToList();

        // Execute through pipeline
        return await ExecutePipeline(request, handler, behaviors, cancellationToken);
    }

    /// <inheritdoc />
    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        // If outbox is enabled, save to outbox instead of publishing directly
        if (_options.OutboxEnabled)
        {
            var outboxWriter = _serviceProvider.GetService<IOutboxWriter>();
            if (outboxWriter != null)
            {
                await outboxWriter.SaveAsync(notification, cancellationToken);
                _logger?.LogDebug(
                    "Saved notification of type {NotificationType} to outbox",
                    notification.GetType().Name);
                return;
            }
        }

        var notificationType = notification.GetType();
        _logger?.LogDebug("Publishing notification of type {NotificationType}", notificationType.Name);

        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        var handlers = _serviceProvider.GetServices(handlerType).Where(h => h != null);

        if (_options.PublishParallelByDefault)
        {
            var tasks = handlers.Select(handler => InvokeNotificationHandler(handler!, notification, cancellationToken));
            await Task.WhenAll(tasks);
        }
        else
        {
            foreach (var handler in handlers)
            {
                await InvokeNotificationHandler(handler!, notification, cancellationToken);
            }
        }
    }

    /// <inheritdoc />
    public async Task PublishParallel(INotification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        // If outbox is enabled, save to outbox instead of publishing directly
        if (_options.OutboxEnabled)
        {
            var outboxWriter = _serviceProvider.GetService<IOutboxWriter>();
            if (outboxWriter != null)
            {
                await outboxWriter.SaveAsync(notification, cancellationToken);
                _logger?.LogDebug(
                    "Saved notification of type {NotificationType} to outbox",
                    notification.GetType().Name);
                return;
            }
        }

        var notificationType = notification.GetType();
        _logger?.LogDebug("Publishing notification of type {NotificationType} in parallel", notificationType.Name);

        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        var handlers = _serviceProvider.GetServices(handlerType).Where(h => h != null);

        var tasks = handlers.Select(handler => InvokeNotificationHandler(handler!, notification, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task<TResponse> ExecutePipeline<TResponse>(
        IRequest<TResponse> request,
        object handler,
        IEnumerable<object> behaviors,
        CancellationToken cancellationToken)
    {
        // Create the handler delegate
        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var handlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handleMethod = handlerInterfaceType.GetMethod("Handle");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handler {handler.GetType().Name} does not implement IRequestHandler correctly");
            }

            var task = handleMethod.Invoke(handler, new object[] { request, cancellationToken }) as Task<TResponse>;
            if (task == null)
            {
                throw new InvalidOperationException($"Handler {handler.GetType().Name} Handle method did not return a Task<TResponse>");
            }

            return task;
        };

        // Build pipeline by wrapping behaviors in reverse order
        var behaviorsList = behaviors.Where(b => b != null).ToList();
        for (int i = behaviorsList.Count - 1; i >= 0; i--)
        {
            var behavior = behaviorsList[i];
            var next = handlerDelegate;

            handlerDelegate = () =>
            {
                var behaviorInterfaceType = typeof(IPipelineBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));
                var behaviorMethod = behaviorInterfaceType.GetMethod("Handle");
                if (behaviorMethod == null)
                {
                    throw new InvalidOperationException($"Behavior {behavior.GetType().Name} does not implement IPipelineBehavior correctly");
                }

                var task = behaviorMethod.Invoke(behavior, new object[] { request, cancellationToken, next }) as Task<TResponse>;
                if (task == null)
                {
                    throw new InvalidOperationException($"Behavior {behavior.GetType().Name} Handle method did not return a Task<TResponse>");
                }

                return task;
            };
        }

        return await handlerDelegate();
    }

    private Task InvokeNotificationHandler(object handler, INotification notification, CancellationToken cancellationToken)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var handlerType = handler.GetType();
        var interfaceType = handlerType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

        if (interfaceType == null)
        {
            throw new InvalidOperationException($"Handler {handlerType.Name} does not implement INotificationHandler");
        }

        var handleMethod = interfaceType.GetMethod("Handle");
        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handler {handlerType.Name} does not have a Handle method");
        }

        var task = handleMethod.Invoke(handler, new object[] { notification, cancellationToken }) as Task;
        if (task == null)
        {
            throw new InvalidOperationException($"Handler {handlerType.Name} Handle method did not return a Task");
        }

        return task;
    }
}
