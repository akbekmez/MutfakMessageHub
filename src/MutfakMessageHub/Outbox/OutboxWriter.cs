using System.Text.Json;
using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.Outbox;

/// <summary>
/// Default implementation of <see cref="IOutboxWriter"/>.
/// </summary>
public class OutboxWriter : IOutboxWriter
{
    private readonly IOutboxStore _store;
    private readonly ILogger<OutboxWriter>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxWriter"/> class.
    /// </summary>
    /// <param name="store">The outbox store.</param>
    /// <param name="logger">Optional logger.</param>
    public OutboxWriter(IOutboxStore store, ILogger<OutboxWriter>? logger = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SaveAsync(INotification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        var notificationType = notification.GetType();
        var message = new OutboxMessage
        {
            NotificationType = notificationType.Name,
            NotificationTypeFullName = notificationType.FullName ?? notificationType.Name,
            Payload = JsonSerializer.Serialize(notification),
            CreatedAt = DateTime.UtcNow
        };

        await _store.SaveAsync(message, cancellationToken);

        _logger?.LogDebug(
            "Saved notification of type {NotificationType} to outbox with ID {MessageId}",
            notificationType.Name,
            message.Id);
    }
}

