using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Behaviors;
using MutfakMessageHub.Configuration;
using MutfakMessageHub.Core;
using MutfakMessageHub.DeadLetterQueue;
using MutfakMessageHub.Outbox;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.DependencyInjection;

/// <summary>
/// Extension methods for setting up MutfakMessageHub services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MutfakMessageHub services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMutfakMessageHub(this IServiceCollection services)
    {
        return services.AddMutfakMessageHub(_ => { });
    }

    /// <summary>
    /// Adds MutfakMessageHub services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configure">An action to configure the <see cref="MutfakMessageHubOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMutfakMessageHub(
        this IServiceCollection services,
        Action<MutfakMessageHubOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var options = new MutfakMessageHubOptions();
        configure(options);

        services.TryAddSingleton(options);
        services.TryAddScoped<IMessageHub, MessageHub>();

        // Register default behaviors based on options
        if (options.CachingEnabled)
        {
            services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(CacheBehavior<,>));
        }

        if (options.RetryEnabled)
        {
            services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
        }

        if (options.TelemetryEnabled)
        {
            services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(TelemetryBehavior<,>));
        }

        // Register outbox components if enabled
        if (options.OutboxEnabled)
        {
            // Register outbox store (default: InMemoryOutboxStore)
            // Users can override this by registering their own IOutboxStore
            services.TryAddSingleton<IOutboxStore, InMemoryOutboxStore>();
            services.TryAddScoped<IOutboxWriter, OutboxWriter>();
            
            // Register outbox processor as background service
            services.AddHostedService<OutboxProcessor>();
        }

        // Register dead-letter queue if enabled
        if (options.DeadLetterQueueEnabled)
        {
            // Register DLQ (default: InMemoryDeadLetterQueue)
            // Users can override this by registering their own IDeadLetterQueue
            services.TryAddSingleton<IDeadLetterQueue, InMemoryDeadLetterQueue>();
        }

        // Always register exception handling and validation behaviors
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds a pipeline behavior to the service collection.
    /// </summary>
    /// <typeparam name="TBehavior">The type of behavior. Must implement IPipelineBehavior&lt;TRequest, TResponse&gt;.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddPipelineBehavior<TBehavior>(this IServiceCollection services)
        where TBehavior : class
    {
        // Register as open generic - the behavior must implement IPipelineBehavior<,>
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(TBehavior));
        return services;
    }
}

