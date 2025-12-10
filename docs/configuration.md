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

Handler discovery şu anda reflection ile yapılmaktadır. Source generator desteği planlanmaktadır ve compile-time handler keşfi ile runtime performansını artıracaktır.

## Open Generic Caching

Tüm handler tipleri ilk çözümlemeden sonra cache edilir.
Bu sayede:
- Daha hızlı resolve
- Daha az GC yükü
- Daha düşük DI maliyeti
sağlanır.
