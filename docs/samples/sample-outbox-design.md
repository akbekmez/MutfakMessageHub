# Outbox Sample

Outbox sistemi şu bileşenlerden oluşur:

- OutboxMessage: DB kaydı
- OutboxWriter: Publish öncesi event'i kaydeder
- OutboxProcessor: Arka planda eventleri alıp Publish eder
- OutboxCleanup: İşlenen eventleri temizler

Bu tasarım, mikroservislerde mesaj kaybını engeller.
