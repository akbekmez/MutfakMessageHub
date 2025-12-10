# Outbox Sample

Outbox sistemi şu bileşenlerden oluşur:

- **OutboxMessage**: DB kaydı (notification'ın serialize edilmiş hali)
- **IOutboxWriter**: Publish öncesi event'i kaydeder
- **OutboxProcessor**: Arka planda eventleri alıp Publish eder (BackgroundService)
- **IOutboxStore**: Outbox mesajlarını saklar ve yönetir
  - `DeleteProcessedAsync()`: İşlenen eventleri temizler

Bu tasarım, mikroservislerde mesaj kaybını engeller.

## Kullanım

``` csharp
// Outbox'ı etkinleştir
services.AddMutfakMessageHub(options =>
{
    options.EnableOutbox();
});

// Publish otomatik olarak outbox'a kaydedilir
await messageHub.Publish(new UserCreatedNotification { UserId = 1 });

// OutboxProcessor arka planda mesajları işler
```
