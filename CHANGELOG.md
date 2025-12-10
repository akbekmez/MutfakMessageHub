# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial project structure
- Request/Response messaging support (MediatR-compatible API)
- Notification broadcasting (sequential and parallel dispatch)
- Pipeline behavior architecture
- Exception handling behavior
- Validation behavior (Data Annotations support)
- Retry behavior (configurable retry attempts and delays)
- Cache behavior (IMemoryCache integration with CacheAttribute)
- Timeout behavior (configurable request timeouts)
- Telemetry behavior (OpenTelemetry Activity support)
- Outbox pattern support (reliable notification delivery)
- Dead-letter queue support (failed handler tracking)
- Open-generic caching for high performance
- Dependency injection integration
- Unit test project with xUnit (18 tests covering core functionality)
- Multi-targeting support (.NET 6.0, 7.0, 8.0, 10.0)
- CI/CD workflows (GitHub Actions)
- NuGet package ready for publishing

### Security
- Updated Microsoft.Extensions.Caching.Memory to 8.0.1 (fixes CVE-2024-43483)
- Updated related dependencies to compatible versions

## [1.0.0] - TBD

### Added
- Initial release

