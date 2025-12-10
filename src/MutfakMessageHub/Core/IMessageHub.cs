using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.Core;

/// <summary>
/// Defines a mediator to encapsulate request/response and publishing interaction patterns.
/// </summary>
public interface IMessageHub
{
    /// <summary>
    /// Asynchronously send a request to a single handler.
    /// </summary>
    /// <typeparam name="TResponse">Response type.</typeparam>
    /// <param name="request">Request object.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously publish a notification to multiple handlers.
    /// </summary>
    /// <param name="notification">Notification object.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that represents the publish operation.</returns>
    Task Publish(INotification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously publish a notification to multiple handlers in parallel.
    /// </summary>
    /// <param name="notification">Notification object.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task that represents the publish operation.</returns>
    Task PublishParallel(INotification notification, CancellationToken cancellationToken = default);
}

