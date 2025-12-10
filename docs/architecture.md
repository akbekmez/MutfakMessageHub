# MutfakMessageHub Architecture

MutfakMessageHub, handler discovery, pipeline behaviors ve message dispatch mekanizmasını
birbirinden ayrılmış bağımsız bileşenlerle sağlar. Mimari üç ana katmana ayrılır:

1. Message Abstractions
   - IRequest<T>, INotification
   - IRequestHandler<,>, INotificationHandler<>
   - RequestHandlerDelegate<T>

2. Execution Pipeline
   - Behavior zinciri
   - Exception, Validation, Retry, Cache, Timeout
   - Sequential veya Parallel dispatch

3. Runtime Engine
   - Handler registry
   - Lazy resolve + open generic caching
   - Outbox worker
   - Telemetry instrumentation

Tüm bu yapı DI container üzerine inşa edilir. Handler discovery şu anda reflection ile yapılmaktadır. Source generator desteği planlanmaktadır.
