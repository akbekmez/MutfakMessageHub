namespace MutfakMessageHub.Outbox;

/// <summary>
/// Defines a store for outbox messages.
/// </summary>
public interface IOutboxStore
{
    /// <summary>
    /// Saves an outbox message to the store.
    /// </summary>
    /// <param name="message">The message to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the save operation.</returns>
    Task SaveAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves unprocessed outbox messages.
    /// </summary>
    /// <param name="batchSize">Maximum number of messages to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of unprocessed messages.</returns>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as processed.
    /// </summary>
    /// <param name="messageId">The ID of the message to mark as processed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the operation.</returns>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as failed.
    /// </summary>
    /// <param name="messageId">The ID of the message.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the operation.</returns>
    Task MarkAsFailedAsync(Guid messageId, string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes processed messages older than the specified date.
    /// </summary>
    /// <param name="olderThan">Delete messages processed before this date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of deleted messages.</returns>
    Task<int> DeleteProcessedAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}

