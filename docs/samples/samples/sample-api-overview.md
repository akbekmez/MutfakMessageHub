# Sample API Overview

Bu örnek API, MutfakMessageHub'ın tipik bir web projesinde nasıl
konumlandığını göstermektedir.

Katmanlar:

- Api (Controllers)
- Application (Requests, Notifications, Behaviors)
- Infrastructure (Outbox, Telemetry, Cache)
- Domain (Entities, Events)

Controller → Send/Publish → Behaviors → Handler → Response
