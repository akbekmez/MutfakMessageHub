using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Core;

namespace MutfakMessageHub.Outbox;

/// <summary>
/// Background service that processes outbox messages.
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOutboxStore _store;
    private readonly MutfakMessageHub.Configuration.MutfakMessageHubOptions _options;
    private readonly ILogger<OutboxProcessor>? _logger;
    private readonly TimeSpan _pollingInterval;
    private readonly int _batchSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxProcessor"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="store">The outbox store.</param>
    /// <param name="options">The options.</param>
    /// <param name="logger">Optional logger.</param>
    /// <param name="pollingInterval">The interval between polling for messages. Default is 5 seconds.</param>
    /// <param name="batchSize">The batch size for processing messages. Default is 100.</param>
    public OutboxProcessor(
        IServiceProvider serviceProvider,
        IOutboxStore store,
        MutfakMessageHub.Configuration.MutfakMessageHubOptions options,
        ILogger<OutboxProcessor>? logger = null,
        TimeSpan? pollingInterval = null,
        int batchSize = 100)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
        _pollingInterval = pollingInterval ?? TimeSpan.FromSeconds(5);
        _batchSize = batchSize;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger?.LogInformation("Outbox processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger?.LogInformation("Outbox processor stopped");
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        var messages = await _store.GetUnprocessedAsync(_batchSize, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await ProcessMessageAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(
                    ex,
                    "Error processing outbox message {MessageId}",
                    message.Id);

                await _store.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            }
        }
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var messageHub = scope.ServiceProvider.GetRequiredService<IMessageHub>();

        // Deserialize notification
        var notificationType = Type.GetType(message.NotificationTypeFullName);
        if (notificationType == null)
        {
            throw new InvalidOperationException(
                $"Could not resolve notification type {message.NotificationTypeFullName}");
        }

        var notification = JsonSerializer.Deserialize(message.Payload, notificationType);
        if (notification == null || notification is not INotification typedNotification)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize notification of type {message.NotificationTypeFullName}");
        }

        // Publish notification
        if (_options.PublishParallelByDefault)
        {
            await messageHub.PublishParallel(typedNotification, cancellationToken);
        }
        else
        {
            await messageHub.Publish(typedNotification, cancellationToken);
        }

        // Mark as processed
        await _store.MarkAsProcessedAsync(message.Id, cancellationToken);

        _logger?.LogDebug(
            "Processed outbox message {MessageId} of type {NotificationType}",
            message.Id,
            message.NotificationType);
    }

    // StartAsync and StopAsync are handled by BackgroundService base class
    // IOutboxProcessor interface methods are not needed when using BackgroundService
}

