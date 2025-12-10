using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Behaviors;

/// <summary>
/// Pipeline behavior that retries failed requests.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<RetryBehavior<TRequest, TResponse>>? _logger;
    private readonly int _maxRetries;
    private readonly TimeSpan _delayBetweenRetries;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="maxRetries">Maximum number of retry attempts. Default is 3.</param>
    /// <param name="delayBetweenRetries">Delay between retry attempts. Default is 1 second.</param>
    /// <param name="logger">Optional logger.</param>
    public RetryBehavior(
        int maxRetries = 3,
        TimeSpan? delayBetweenRetries = null,
        ILogger<RetryBehavior<TRequest, TResponse>>? logger = null)
    {
        _maxRetries = maxRetries > 0 ? maxRetries : 3;
        _delayBetweenRetries = delayBetweenRetries ?? TimeSpan.FromSeconds(1);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var attempt = 0;
        Exception? lastException = null;

        while (attempt <= _maxRetries)
        {
            try
            {
                return await next();
            }
            catch (Exception ex) when (attempt < _maxRetries && ShouldRetry(ex))
            {
                attempt++;
                lastException = ex;
                _logger?.LogWarning(
                    ex,
                    "Request of type {RequestType} failed on attempt {Attempt}. Retrying in {Delay}ms...",
                    typeof(TRequest).Name,
                    attempt,
                    _delayBetweenRetries.TotalMilliseconds);

                await Task.Delay(_delayBetweenRetries, cancellationToken);
            }
        }

        _logger?.LogError(
            lastException,
            "Request of type {RequestType} failed after {Attempts} attempts",
            typeof(TRequest).Name,
            attempt);

        throw lastException ?? new InvalidOperationException("Request failed but no exception was captured");
    }

    /// <summary>
    /// Determines if an exception should trigger a retry.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>True if the request should be retried, false otherwise.</returns>
    protected virtual bool ShouldRetry(Exception exception)
    {
        // Retry on transient exceptions (network issues, timeouts, etc.)
        // Override this method to customize retry logic
        return exception is TimeoutException
            || (exception.InnerException != null && ShouldRetry(exception.InnerException));
    }
}

