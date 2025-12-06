# MutfakMessageHub Configuration

## Basic Registration

services.AddMutfakMessageHub();

## Enable Subsystems

services.AddMutfakMessageHub(o =>
{
    o.EnableCaching();
    o.EnableRetry();
    o.EnableOutbox();
    o.EnableTelemetry();
    o.EnableDeadLetterQueue();
    o.PublishParallelByDefault = false;
});

## Handler Discovery

Source generator otomatik olarak tüm IRequestHandler ve
INotificationHandler implementasyonlarını işler ve runtime performansını artırır.

## Open Generic Caching

Tüm handler tipleri ilk çözümlemeden sonra cache edilir.
Bu sayede:
- Daha hızlı resolve
- Daha az GC yükü
- Daha düşük DI maliyeti
sağlanır.
