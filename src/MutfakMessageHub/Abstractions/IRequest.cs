namespace MutfakMessageHub.Abstractions;

/// <summary>
/// Marker interface for request messages that return a response.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the request.</typeparam>
public interface IRequest<out TResponse>
{
}

