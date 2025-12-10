using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.DeadLetterQueue;

/// <summary>
/// In-memory implementation of <see cref="IDeadLetterQueue"/>.
/// Note: This is a simple implementation for development/testing. For production, use a persistent store.
/// </summary>
public class InMemoryDeadLetterQueue : IDeadLetterQueue
{
    private readonly ConcurrentDictionary<Guid, DeadLetterMessage> _messages = new();
    private readonly ILogger<InMemoryDeadLetterQueue>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryDeadLetterQueue"/> class.
    /// </summary>
    /// <param name="logger">Optional logger.</param>
    public InMemoryDeadLetterQueue(ILogger<InMemoryDeadLetterQueue>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task AddAsync(
        INotification notification,
        Type handlerType,
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        if (handlerType == null)
        {
            throw new ArgumentNullException(nameof(handlerType));
        }

        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var notificationType = notification.GetType();
        var message = new DeadLetterMessage
        {
            NotificationType = notificationType.Name,
            NotificationTypeFullName = notificationType.FullName ?? notificationType.Name,
            Payload = JsonSerializer.Serialize(notification),
            HandlerType = handlerType.FullName ?? handlerType.Name,
            ErrorMessage = exception.Message,
            StackTrace = exception.StackTrace,
            CreatedAt = DateTime.UtcNow
        };

        _messages.TryAdd(message.Id, message);

        _logger?.LogWarning(
            "Added failed notification handler to dead-letter queue. Notification: {NotificationType}, Handler: {HandlerType}, Error: {ErrorMessage}",
            notificationType.Name,
            handlerType.Name,
            exception.Message);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IEnumerable<DeadLetterMessage>> GetMessagesAsync(
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        var messages = _messages.Values
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IEnumerable<DeadLetterMessage>>(messages);
    }

    /// <inheritdoc />
    public Task DeleteAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        _messages.TryRemove(messageId, out _);
        return Task.CompletedTask;
    }
}

