using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Behaviors;
using MutfakMessageHub.Pipeline;
using MutfakMessageHub.Tests.Core;
using Xunit;

namespace MutfakMessageHub.Tests.Behaviors;

public class ExceptionHandlingBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNextThrowsException_LogsAndRethrows()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<TestRequest, string>>>();
        var behavior = new ExceptionHandlingBehavior<TestRequest, string>(loggerMock.Object);

        var request = new TestRequest();
        var expectedException = new InvalidOperationException("Test exception");

        RequestHandlerDelegate<string> next = () => throw expectedException;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => behavior.Handle(request, CancellationToken.None, next));

        Assert.Equal(expectedException, exception);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNextSucceeds_ReturnsResult()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingBehavior<TestRequest, string>>>();
        var behavior = new ExceptionHandlingBehavior<TestRequest, string>(loggerMock.Object);

        var request = new TestRequest();
        var expectedResult = "Success";

        RequestHandlerDelegate<string> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}

