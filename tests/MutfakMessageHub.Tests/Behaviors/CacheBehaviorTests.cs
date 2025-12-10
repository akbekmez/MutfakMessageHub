using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Attributes;
using MutfakMessageHub.Behaviors;
using MutfakMessageHub.Pipeline;
using MutfakMessageHub.Tests.Core;
using Xunit;

namespace MutfakMessageHub.Tests.Behaviors;

public class CacheBehaviorTests
{
    [Fact]
    public async Task Handle_WithoutCache_DoesNotCache()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CacheBehavior<TestRequest, string>>>();
        var behavior = new CacheBehavior<TestRequest, string>(cache: null, logger: loggerMock.Object);

        var request = new TestRequest { Message = "Test" };
        var callCount = 0;

        RequestHandlerDelegate<string> next = () =>
        {
            callCount++;
            return Task.FromResult("Result");
        };

        // Act
        await behavior.Handle(request, CancellationToken.None, next);
        await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.Equal(2, callCount); // Should call twice without cache
    }

    [Fact]
    public async Task Handle_WithCacheAndNoAttribute_DoesNotCache()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var loggerMock = new Mock<ILogger<CacheBehavior<TestRequest, string>>>();
        var behavior = new CacheBehavior<TestRequest, string>(cache, logger: loggerMock.Object);

        var request = new TestRequest { Message = "Test" };
        var callCount = 0;

        RequestHandlerDelegate<string> next = () =>
        {
            callCount++;
            return Task.FromResult("Result");
        };

        // Act
        await behavior.Handle(request, CancellationToken.None, next);
        await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.Equal(2, callCount); // Should call twice without Cache attribute
    }

    [Fact]
    public async Task Handle_WithCacheAndAttribute_CachesResult()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var loggerMock = new Mock<ILogger<CacheBehavior<CachedRequest, string>>>();
        var behavior = new CacheBehavior<CachedRequest, string>(cache, logger: loggerMock.Object);

        var request = new CachedRequest { Message = "Test" };
        var callCount = 0;

        RequestHandlerDelegate<string> next = () =>
        {
            callCount++;
            return Task.FromResult("Result");
        };

        // Act
        var result1 = await behavior.Handle(request, CancellationToken.None, next);
        var result2 = await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.Equal("Result", result1);
        Assert.Equal("Result", result2);
        Assert.Equal(1, callCount); // Should only call once, second call should use cache
    }
}

[Cache(DurationSeconds = 60)]
public class CachedRequest : IRequest<string>
{
    public string Message { get; set; } = string.Empty;
}

