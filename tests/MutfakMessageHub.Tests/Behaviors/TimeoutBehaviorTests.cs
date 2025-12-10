using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Behaviors;
using MutfakMessageHub.Pipeline;
using MutfakMessageHub.Tests.Core;
using Xunit;

namespace MutfakMessageHub.Tests.Behaviors;

public class TimeoutBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNextCompletesInTime_ReturnsResult()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TimeoutBehavior<TestRequest, string>>>();
        var behavior = new TimeoutBehavior<TestRequest, string>(
            timeout: TimeSpan.FromSeconds(5),
            logger: loggerMock.Object);

        var request = new TestRequest();
        var expectedResult = "Success";

        RequestHandlerDelegate<string> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task Handle_WhenNextExceedsTimeout_ThrowsTimeoutException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TimeoutBehavior<TestRequest, string>>>();
        var behavior = new TimeoutBehavior<TestRequest, string>(
            timeout: TimeSpan.FromMilliseconds(50),
            logger: loggerMock.Object);

        var request = new TestRequest();

        RequestHandlerDelegate<string> next = async () =>
        {
            await Task.Delay(200, CancellationToken.None);
            return "Success";
        };

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(
            () => behavior.Handle(request, CancellationToken.None, next));
    }

    [Fact]
    public async Task Handle_WhenCancellationTokenCancelled_ThrowsOperationCanceledException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<TimeoutBehavior<TestRequest, string>>>();
        var behavior = new TimeoutBehavior<TestRequest, string>(
            timeout: TimeSpan.FromSeconds(5),
            logger: loggerMock.Object);

        var request = new TestRequest();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        RequestHandlerDelegate<string> next = async () =>
        {
            await Task.Delay(100, cts.Token);
            return "Success";
        };

        // Act & Assert
        // TaskCanceledException is derived from OperationCanceledException
        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => behavior.Handle(request, cts.Token, next));
        Assert.True(exception is TaskCanceledException || exception is OperationCanceledException);
    }
}

