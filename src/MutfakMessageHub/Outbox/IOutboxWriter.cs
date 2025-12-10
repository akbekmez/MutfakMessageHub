using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.Outbox;

/// <summary>
/// Defines a writer for storing notifications in the outbox.
/// </summary>
public interface IOutboxWriter
{
    /// <summary>
    /// Saves a notification to the outbox for reliable delivery.
    /// </summary>
    /// <param name="notification">The notification to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the save operation.</returns>
    Task SaveAsync(INotification notification, CancellationToken cancellationToken = default);
}

