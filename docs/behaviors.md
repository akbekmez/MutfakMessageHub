# Pipeline Behaviors

MutfakMessageHub, tüm request ve notification akışını davranış zinciri üzerinden
yönlendiren bir pipeline mimarisi kullanır.

## Default Behaviors

- ExceptionHandlingBehavior
- ValidationBehavior
- RetryBehavior
- CacheBehavior
- TimeoutBehavior
- TelemetryBehavior
- OutboxBehavior

Her behavior şu imzayı takip eder:

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        CancellationToken token,
        RequestHandlerDelegate<TResponse> next);
}

next() çağrısı zincirin bir sonraki halkasını temsil eder.
