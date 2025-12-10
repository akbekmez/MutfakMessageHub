using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Sample.Common.Behaviors;

/// <summary>
/// Custom logging behavior to demonstrate custom pipeline behaviors.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var requestType = typeof(TRequest).Name;
        _logger.LogInformation("Handling request: {RequestType}", requestType);

        try
        {
            var response = await next();
            _logger.LogInformation("Request {RequestType} completed successfully", requestType);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request {RequestType} failed", requestType);
            throw;
        }
    }
}

