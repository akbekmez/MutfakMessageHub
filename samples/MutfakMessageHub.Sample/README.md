# MutfakMessageHub Sample Application

Bu örnek uygulama, MutfakMessageHub kütüphanesinin tüm özelliklerini gösteren bir ASP.NET Core Web API projesidir.

## Özellikler

Bu demo uygulaması aşağıdaki özellikleri gösterir:

- ✅ **Request/Response Messaging**: `GetUserQuery`, `CreateUserCommand`, `GetProductsQuery`
- ✅ **Notification Broadcasting**: `UserCreatedNotification` (sequential ve parallel)
- ✅ **Pipeline Behaviors**:
  - Validation (Data Annotations)
  - Caching (`[Cache]` attribute)
  - Timeout (`[RequestTimeout]` attribute)
  - Retry (automatic)
  - Exception Handling (automatic)
  - Custom Logging Behavior
- ✅ **Outbox Pattern**: Notification'lar outbox'a kaydedilir
- ✅ **Dead-Letter Queue**: Başarısız notification'lar DLQ'ya yazılır
- ✅ **Telemetry**: OpenTelemetry entegrasyonu

## Çalıştırma

```bash
cd samples/MutfakMessageHub.Sample
dotnet run
```

Uygulama `https://localhost:5001` veya `http://localhost:5000` adresinde çalışacaktır.

Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Users

- `GET /api/users/{id}` - Kullanıcı getir
- `POST /api/users` - Yeni kullanıcı oluştur (sequential notification)
- `POST /api/users/parallel` - Yeni kullanıcı oluştur (parallel notification)

### Products

- `GET /api/products` - Tüm ürünleri getir (cached)
- `GET /api/products/slow` - Timeout test endpoint

### Dead-Letter Queue

- `GET /api/deadletterqueue` - Başarısız mesajları listele

## Örnek Kullanımlar

### 1. Request/Response

```bash
# Get user
curl -X GET "https://localhost:5001/api/users/1"

# Create user (with validation)
curl -X POST "https://localhost:5001/api/users" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test@example.com"}'
```

### 2. Notification (Sequential)

```bash
curl -X POST "https://localhost:5001/api/users" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test@example.com"}'
```

Notification'lar sırayla işlenir:
1. SendWelcomeEmailHandler
2. UpdateUserStatisticsHandler

### 3. Notification (Parallel)

```bash
curl -X POST "https://localhost:5001/api/users/parallel" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test@example.com"}'
```

Notification'lar paralel olarak işlenir.

### 4. Caching

```bash
# İlk istek - cache'den değil
curl -X GET "https://localhost:5001/api/products"

# İkinci istek - cache'den (60 saniye içinde)
curl -X GET "https://localhost:5001/api/products"
```

### 5. Timeout

```bash
# 2 saniye timeout ile test
curl -X GET "https://localhost:5001/api/products/slow"
```

### 6. Dead-Letter Queue

```bash
# Başarısız mesajları görüntüle
curl -X GET "https://localhost:5001/api/deadletterqueue"
```

## Pipeline Behaviors

### Validation Behavior

`CreateUserCommand` sınıfında Data Annotations kullanılmıştır:

```csharp
[Required]
[StringLength(100, MinimumLength = 2)]
public string Name { get; set; }

[Required]
[EmailAddress]
public string Email { get; set; }
```

### Cache Behavior

`GetProductsQuery` sınıfında `[Cache]` attribute kullanılmıştır:

```csharp
[Cache(DurationSeconds = 60)]
public class GetProductsQuery : IRequest<List<ProductDto>>
```

### Timeout Behavior

`SlowQuery` sınıfında `[RequestTimeout]` attribute kullanılmıştır:

```csharp
[RequestTimeout(2000)] // 2 seconds
public class SlowQuery : IRequest<string>
```

### Custom Behavior

`LoggingBehavior` özel bir pipeline behavior örneğidir ve tüm request'leri loglar.

## Yapılandırma

`Program.cs` dosyasında MutfakMessageHub yapılandırması:

```csharp
builder.Services.AddMutfakMessageHub(options =>
{
    options.EnableCaching();
    options.EnableRetry();
    options.EnableOutbox();
    options.EnableTelemetry();
    options.EnableDeadLetterQueue();
    options.PublishParallelByDefault = false;
});
```

## Notlar

- Bu demo uygulaması in-memory veri kullanır (gerçek veritabanı yok)
- Outbox pattern için `InMemoryOutboxStore` kullanılır
- Dead-Letter Queue için `InMemoryDeadLetterQueue` kullanılır
- Production ortamında persistent store kullanılmalıdır

