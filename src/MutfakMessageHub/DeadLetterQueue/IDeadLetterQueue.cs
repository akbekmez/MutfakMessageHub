using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.DeadLetterQueue;

/// <summary>
/// Defines a dead-letter queue for storing failed notification handler executions.
/// </summary>
public interface IDeadLetterQueue
{
    /// <summary>
    /// Adds a failed notification handler execution to the dead-letter queue.
    /// </summary>
    /// <param name="notification">The notification that failed.</param>
    /// <param name="handlerType">The type of handler that failed.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the operation.</returns>
    Task AddAsync(
        INotification notification,
        Type handlerType,
        Exception exception,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves dead-letter messages.
    /// </summary>
    /// <param name="skip">Number of messages to skip.</param>
    /// <param name="take">Maximum number of messages to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of dead-letter messages.</returns>
    Task<IEnumerable<DeadLetterMessage>> GetMessagesAsync(
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a dead-letter message.
    /// </summary>
    /// <param name="messageId">The ID of the message to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the operation.</returns>
    Task DeleteAsync(Guid messageId, CancellationToken cancellationToken = default);
}

