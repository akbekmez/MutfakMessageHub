using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Behaviors;

/// <summary>
/// Pipeline behavior that enforces a timeout on request processing.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public class TimeoutBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TimeoutBehavior<TRequest, TResponse>>? _logger;
    private readonly TimeSpan _timeout;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="timeout">The timeout duration. Default is 30 seconds.</param>
    /// <param name="logger">Optional logger.</param>
    public TimeoutBehavior(
        TimeSpan? timeout = null,
        ILogger<TimeoutBehavior<TRequest, TResponse>>? logger = null)
    {
        _timeout = timeout ?? TimeSpan.FromSeconds(30);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        using var timeoutCts = new CancellationTokenSource(_timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            return await next();
        }
        catch (OperationCanceledException ex) when (timeoutCts.Token.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            _logger?.LogWarning(
                "Request of type {RequestType} timed out after {Timeout}ms",
                typeof(TRequest).Name,
                _timeout.TotalMilliseconds);

            throw new TimeoutException(
                $"Request of type {typeof(TRequest).Name} timed out after {_timeout.TotalMilliseconds}ms",
                ex);
        }
    }
}

