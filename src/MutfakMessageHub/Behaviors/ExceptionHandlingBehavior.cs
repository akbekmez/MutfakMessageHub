using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Behaviors;

/// <summary>
/// Pipeline behavior that handles exceptions thrown during request processing.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">Optional logger.</param>
    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred while handling request of type {RequestType}", typeof(TRequest).Name);
            throw;
        }
    }
}

