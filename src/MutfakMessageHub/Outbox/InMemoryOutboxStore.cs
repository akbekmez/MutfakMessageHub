using System.Collections.Concurrent;

namespace MutfakMessageHub.Outbox;

/// <summary>
/// In-memory implementation of <see cref="IOutboxStore"/>.
/// Note: This is a simple implementation for development/testing. For production, use a persistent store.
/// </summary>
public class InMemoryOutboxStore : IOutboxStore
{
    private readonly ConcurrentDictionary<Guid, OutboxMessage> _messages = new();

    /// <inheritdoc />
    public Task SaveAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _messages.AddOrUpdate(message.Id, message, (_, _) => message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        var unprocessed = _messages.Values
            .Where(m => !m.IsProcessed)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToList();

        return Task.FromResult<IEnumerable<OutboxMessage>>(unprocessed);
    }

    /// <inheritdoc />
    public Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        if (_messages.TryGetValue(messageId, out var message))
        {
            message.IsProcessed = true;
            message.ProcessedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task MarkAsFailedAsync(Guid messageId, string errorMessage, CancellationToken cancellationToken = default)
    {
        if (_messages.TryGetValue(messageId, out var message))
        {
            message.ErrorMessage = errorMessage;
            message.Attempts++;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> DeleteProcessedAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var keysToDelete = _messages.Values
            .Where(m => m.IsProcessed && m.ProcessedAt.HasValue && m.ProcessedAt.Value < olderThan)
            .Select(m => m.Id)
            .ToList();

        var deleted = 0;
        foreach (var key in keysToDelete)
        {
            if (_messages.TryRemove(key, out _))
            {
                deleted++;
            }
        }

        return Task.FromResult(deleted);
    }
}

