using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Behaviors;

/// <summary>
/// Pipeline behavior that adds telemetry instrumentation using System.Diagnostics.Activity.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public class TelemetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ActivitySource ActivitySource = new("MutfakMessageHub");
    private readonly ILogger<TelemetryBehavior<TRequest, TResponse>>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">Optional logger.</param>
    public TelemetryBehavior(ILogger<TelemetryBehavior<TRequest, TResponse>>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var requestTypeName = typeof(TRequest).Name;
        var activityName = $"MutfakMessageHub.Handle.{requestTypeName}";

        using var activity = ActivitySource.StartActivity(activityName);
        
        if (activity != null)
        {
            activity.SetTag("messaging.system", "MutfakMessageHub");
            activity.SetTag("messaging.operation", "handle");
            activity.SetTag("messaging.request.type", requestTypeName);
            activity.SetTag("messaging.request.full_type", typeof(TRequest).FullName);
        }

        _logger?.LogDebug("Starting request handling for {RequestType}", requestTypeName);

        var startTime = DateTime.UtcNow;
        Exception? exception = null;

        try
        {
            var response = await next();
            
            var duration = DateTime.UtcNow - startTime;
            activity?.SetTag("messaging.duration_ms", duration.TotalMilliseconds);
            activity?.SetTag("messaging.success", true);
            
            _logger?.LogDebug(
                "Completed request handling for {RequestType} in {Duration}ms",
                requestTypeName,
                duration.TotalMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            exception = ex;
            var duration = DateTime.UtcNow - startTime;
            
            activity?.SetTag("messaging.duration_ms", duration.TotalMilliseconds);
            activity?.SetTag("messaging.success", false);
            activity?.SetTag("messaging.error", true);
            activity?.SetTag("messaging.error.type", ex.GetType().Name);
            activity?.SetTag("messaging.error.message", ex.Message);
            
            if (activity != null && ex.StackTrace != null)
            {
                activity.SetTag("messaging.error.stack_trace", ex.StackTrace);
            }

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            _logger?.LogError(
                ex,
                "Request handling failed for {RequestType} after {Duration}ms",
                requestTypeName,
                duration.TotalMilliseconds);

            throw;
        }
    }
}

