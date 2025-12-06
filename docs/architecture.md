# MutfakMessageHub Architecture

MutfakMessageHub, handler discovery, pipeline behaviors ve message dispatch mekanizmasını
birbirinden ayrılmış bağımsız bileşenlerle sağlar. Mimari üç ana katmana ayrılır:

1. Message Abstractions
   - IRequest<T>, INotification
   - IRequestHandler<,>, INotificationHandler<>
   - RequestHandlerDelegate<T>, NotificationHandlerDelegate

2. Execution Pipeline
   - Behavior zinciri
   - Exception, Validation, Retry, Cache, Timeout
   - Sequential veya Parallel dispatch

3. Runtime Engine
   - Handler registry
   - Lazy resolve + open generic caching
   - Outbox worker
   - Telemetry instrumentation

Tüm bu yapı DI container üzerine inşa edilir ve reflection gerektiren operasyonlar
source generator ile minimize edilir.
