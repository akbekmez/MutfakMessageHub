using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Behaviors;
using MutfakMessageHub.Pipeline;
using MutfakMessageHub.Tests.Core;
using Xunit;

namespace MutfakMessageHub.Tests.Behaviors;

public class RetryBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNextSucceeds_ReturnsResult()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryBehavior<TestRequest, string>>>();
        var behavior = new RetryBehavior<TestRequest, string>(logger: loggerMock.Object);

        var request = new TestRequest();
        var expectedResult = "Success";

        RequestHandlerDelegate<string> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task Handle_WhenNextThrowsTimeoutException_Retries()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryBehavior<TestRequest, string>>>();
        var behavior = new RetryBehavior<TestRequest, string>(
            maxRetries: 2,
            delayBetweenRetries: TimeSpan.FromMilliseconds(10),
            logger: loggerMock.Object);

        var request = new TestRequest();
        var attemptCount = 0;

        RequestHandlerDelegate<string> next = () =>
        {
            attemptCount++;
            if (attemptCount < 3)
            {
                throw new TimeoutException("Timeout");
            }
            return Task.FromResult("Success");
        };

        // Act
        var result = await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.Equal("Success", result);
        Assert.Equal(3, attemptCount);
    }

    [Fact]
    public async Task Handle_WhenMaxRetriesExceeded_ThrowsException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryBehavior<TestRequest, string>>>();
        var behavior = new RetryBehavior<TestRequest, string>(
            maxRetries: 2,
            delayBetweenRetries: TimeSpan.FromMilliseconds(10),
            logger: loggerMock.Object);

        var request = new TestRequest();
        var expectedException = new TimeoutException("Timeout");

        RequestHandlerDelegate<string> next = () => throw expectedException;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TimeoutException>(
            () => behavior.Handle(request, CancellationToken.None, next));

        Assert.Equal(expectedException, exception);
    }

    [Fact]
    public async Task Handle_WhenNextThrowsNonRetryableException_DoesNotRetry()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<RetryBehavior<TestRequest, string>>>();
        var behavior = new RetryBehavior<TestRequest, string>(
            maxRetries: 2,
            delayBetweenRetries: TimeSpan.FromMilliseconds(10),
            logger: loggerMock.Object);

        var request = new TestRequest();
        var expectedException = new InvalidOperationException("Not retryable");
        var attemptCount = 0;

        RequestHandlerDelegate<string> next = () =>
        {
            attemptCount++;
            throw expectedException;
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => behavior.Handle(request, CancellationToken.None, next));

        Assert.Equal(expectedException, exception);
        Assert.Equal(1, attemptCount); // Should not retry
    }
}

