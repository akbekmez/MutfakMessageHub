using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Outbox;
using MutfakMessageHub.Tests.Core;
using Xunit;

namespace MutfakMessageHub.Tests.Outbox;

public class OutboxWriterTests
{
    [Fact]
    public async Task SaveAsync_WithValidNotification_SavesToStore()
    {
        // Arrange
        var storeMock = new Mock<IOutboxStore>();
        var loggerMock = new Mock<ILogger<OutboxWriter>>();
        var writer = new OutboxWriter(storeMock.Object, loggerMock.Object);

        var notification = new TestNotification { Message = "Test" };

        // Act
        await writer.SaveAsync(notification);

        // Assert
        storeMock.Verify(
            x => x.SaveAsync(
                It.Is<OutboxMessage>(m =>
                    m.NotificationType == nameof(TestNotification) &&
                    !string.IsNullOrEmpty(m.Payload)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WithNullNotification_ThrowsArgumentNullException()
    {
        // Arrange
        var storeMock = new Mock<IOutboxStore>();
        var loggerMock = new Mock<ILogger<OutboxWriter>>();
        var writer = new OutboxWriter(storeMock.Object, loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => writer.SaveAsync(null!));
    }
}

