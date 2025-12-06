<p align="center">
  <img src="https://mutfakyazlimevi.com/.../logo.png" width="180" />
</p>

![License](https://img.shields.io/badge/license-MIT-blue)

![NuGet](https://img.shields.io/badge/NuGet-coming_soon-yellow)

![Build](https://img.shields.io/badge/build-passing-brightgreen) 

### 🔥 Highlights
- MediatR-compatible API
- Request/Response messaging
- Notification broadcasting
- Parallel & Sequential dispatch
- Pipeline behaviors (Exception, Validation, Retry, Cache)
- Outbox pattern
- Telemetry (OpenTelemetry)
- Dead-letter queue
- Timeout support
- Source generator (handler discovery)
- High-performance open-generic caching


**MutfakMessageHub**, .NET uygulamaları için modern, hafif ve
genişletilebilir bir in-process messaging kütüphanesidir. MediatR'ın
kullanım kolaylığını korurken; pipeline, performans, telemetri ve mesaj
güvenilirliğini artıran ek özellikler içerir.

Kütüphane tam olarak aşağıdaki bileşenleri destekler:

-   Request/Response messaging
-   Notification broadcasting (sequential veya parallel dispatch)
-   Pipeline behavior zinciri
-   Exception, Validation, Retry ve Caching behavior'ları
-   Outbox Pattern ile güvenilir mesaj teslimi
-   OpenTelemetry entegrasyonu
-   Source Generator tabanlı handler discovery
-   Timeout ve Dead-Letter mekanizması

## Özellikler

### ✓ Request / Response Modeli

MediatR ile birebir uyumlu API:

``` csharp
public interface IRequest<T> { }
public interface IRequestHandler<TRequest, TResponse> { }
```

### ✓ Notification Modeli

Broadcast tarzı event işleme:

``` csharp
public interface INotification { }
public interface INotificationHandler<TNotification> { }
```

Sequential ya da parallel publish desteklenir.

### ✓ Pipeline Architecture

Hem request hem notification için pipeline behavior desteği:

-   ExceptionHandlingBehavior
-   ValidationBehavior
-   RetryBehavior
-   CachingBehavior
-   UnitOfWorkBehavior (opsiyonel)
-   TimeoutBehavior
-   Custom behavior desteği

## Kurulum

``` bash
dotnet add package MutfakMessageHub
```

``` csharp
services.AddMutfakMessageHub(options =>
{
    options.EnableCaching();
    options.EnableRetry();
    options.EnableOutbox();
    options.EnableTelemetry();
    options.EnableDeadLetterQueue();
});
```

## Kullanım

### Request / Handler

``` csharp
public class GetUserQuery : IRequest<UserDto>
{
    public int Id { get; set; }
}

public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserQuery request, CancellationToken token)
    {
        return Task.FromResult(new UserDto { Id = request.Id });
    }
}
```

### Notification / Handler

``` csharp
public class UserCreatedNotification : INotification
{
    public int UserId { get; set; }
}

public class SendWelcomeMailHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}
```

### Publish

``` csharp
await mediator.Publish(new UserCreatedNotification { UserId = 15 });
```

Parallel publish:

``` csharp
await mediator.PublishParallel(new UserCreatedNotification { UserId = 15 });
```

## Pipeline Behavior

### Exception Handling

``` csharp
public class ExceptionHandlingBehavior<TRequest, TResponse>
    : IMessagePipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken token,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
```

### Cache Behavior

``` csharp
[Cache(DurationSeconds = 60)]
public class GetProductsQuery : IRequest<List<Product>> { }
```

## Outbox Pattern

Dağıtık sistemlerde kayıpsız event yayını sağlar.

## Telemetry

OpenTelemetry ile Activity ve metrik üretir.

## Source Generator

Compile-time handler keşfi ile yüksek performans sağlar.

## Timeout Behavior

``` csharp
[RequestTimeout(2000)]
public class SlowQuery : IRequest<string> { }
```

## Dead-Letter Queue

Başarısız notification handler sonuçları DLQ'ya yazılır.

## Performans

Open-generic caching, source generator, lazy resolution, minimal
reflection kullanır.

## Lisans

MIT License
