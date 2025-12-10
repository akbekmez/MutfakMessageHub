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

**Not**: Outbox pattern, MessageHub.Publish() metodu içinde direkt olarak implement edilmiştir. Ayrı bir behavior olarak değil, outbox enabled olduğunda otomatik olarak devreye girer.

Her behavior şu imzayı takip eder:

using MutfakMessageHub.Pipeline;
using MutfakMessageHub.Abstractions;

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next);
}

next() çağrısı zincirin bir sonraki halkasını temsil eder.
