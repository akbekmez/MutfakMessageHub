<p align="center">
  <img src="https://mutfakyazlimevi.com/.../logo.png" width="180" />
</p>

![License](https://img.shields.io/badge/license-MIT-blue)

![NuGet](https://img.shields.io/nuget/v/MutfakMessageHub)

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
- Source generator (handler discovery - planned)
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
-   Source Generator tabanlı handler discovery (planned)
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
-   TimeoutBehavior
-   TelemetryBehavior
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

### Dependency Injection

``` csharp
// Program.cs veya Startup.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMutfakMessageHub(options =>
{
    options.EnableCaching();
    options.EnableRetry();
    options.EnableOutbox();
    options.EnableTelemetry();
    options.EnableDeadLetterQueue();
});

var app = builder.Build();

// IMessageHub'ı inject edin
var messageHub = app.Services.GetRequiredService<IMessageHub>();
```

### Request / Handler

``` csharp
using MutfakMessageHub.Abstractions;

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

// Request gönderme
var messageHub = serviceProvider.GetRequiredService<IMessageHub>();
var user = await messageHub.Send(new GetUserQuery { Id = 5 });
```

### Notification / Handler

``` csharp
using MutfakMessageHub.Abstractions;

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
var messageHub = serviceProvider.GetRequiredService<IMessageHub>();
await messageHub.Publish(new UserCreatedNotification { UserId = 15 });
```

Parallel publish:

``` csharp
await messageHub.PublishParallel(new UserCreatedNotification { UserId = 15 });
```

## Pipeline Behavior

### Exception Handling

``` csharp
using MutfakMessageHub.Pipeline;

public class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
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
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Attributes;

[Cache(DurationSeconds = 60)]
public class GetProductsQuery : IRequest<List<Product>> { }
```

### Validation Behavior

``` csharp
using System.ComponentModel.DataAnnotations;
using MutfakMessageHub.Abstractions;

public class CreateUserCommand : IRequest<UserDto>
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [EmailAddress]
    public string Email { get; set; }
}
```

## Outbox Pattern

Dağıtık sistemlerde kayıpsız event yayını sağlar.

## Telemetry

OpenTelemetry ile Activity ve metrik üretir. TelemetryBehavior otomatik olarak tüm request ve notification'ları izler.

``` csharp
// OpenTelemetry setup (opsiyonel)
services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddSource("MutfakMessageHub")
        .AddConsoleExporter());
```

## Source Generator

Compile-time handler keşfi ile yüksek performans sağlar. (Planned - şu anda reflection kullanılıyor)

## Timeout Behavior

``` csharp
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Attributes;

[RequestTimeout(2000)]
public class SlowQuery : IRequest<string> { }
```

## Dead-Letter Queue

Başarısız notification handler sonuçları DLQ'ya yazılır.

``` csharp
// DLQ'dan başarısız mesajları okuma
var dlq = serviceProvider.GetRequiredService<IDeadLetterQueue>();
var failedMessages = await dlq.GetFailedMessagesAsync();

foreach (var message in failedMessages)
{
    // Başarısız mesajları işle
    Console.WriteLine($"Failed: {message.NotificationType} - {message.ErrorMessage}");
}
```

## Performans

Open-generic caching, source generator, lazy resolution, minimal
reflection kullanır.

## Yayınlama

NuGet.org'a yayınlama adımları için [Publishing Guide](docs/publishing.md) dokümantasyonuna bakın.

## Lisans

MIT License
