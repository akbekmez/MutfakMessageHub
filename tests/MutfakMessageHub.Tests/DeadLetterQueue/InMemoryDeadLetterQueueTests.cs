using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.DeadLetterQueue;
using MutfakMessageHub.Tests.Core;
using Xunit;

namespace MutfakMessageHub.Tests.DeadLetterQueue;

public class InMemoryDeadLetterQueueTests
{
    [Fact]
    public async Task AddAsync_WithValidData_AddsMessage()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<InMemoryDeadLetterQueue>>();
        var dlq = new InMemoryDeadLetterQueue(loggerMock.Object);
        var notification = new TestNotification { Message = "Test" };
        var handlerType = typeof(TestNotificationHandler);
        var exception = new InvalidOperationException("Test error");

        // Act
        await dlq.AddAsync(notification, handlerType, exception);

        // Assert
        var messages = await dlq.GetMessagesAsync();
        Assert.Single(messages);
        var message = messages.First();
        Assert.Equal(nameof(TestNotification), message.NotificationType);
        Assert.Equal(handlerType.FullName, message.HandlerType);
        Assert.Equal("Test error", message.ErrorMessage);
    }

    [Fact]
    public async Task AddAsync_WithNullNotification_ThrowsArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<InMemoryDeadLetterQueue>>();
        var dlq = new InMemoryDeadLetterQueue(loggerMock.Object);
        var handlerType = typeof(TestNotificationHandler);
        var exception = new Exception("Test");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => dlq.AddAsync(null!, handlerType, exception));
    }

    [Fact]
    public async Task GetMessagesAsync_WithPagination_ReturnsCorrectMessages()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<InMemoryDeadLetterQueue>>();
        var dlq = new InMemoryDeadLetterQueue(loggerMock.Object);
        var handlerType = typeof(TestNotificationHandler);
        var exception = new Exception("Test");

        for (int i = 0; i < 5; i++)
        {
            await dlq.AddAsync(new TestNotification { Message = $"Test{i}" }, handlerType, exception);
            await Task.Delay(10); // Ensure different timestamps
        }

        // Act
        var messages = await dlq.GetMessagesAsync(skip: 1, take: 2);

        // Assert
        Assert.Equal(2, messages.Count());
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesMessage()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<InMemoryDeadLetterQueue>>();
        var dlq = new InMemoryDeadLetterQueue(loggerMock.Object);
        var notification = new TestNotification { Message = "Test" };
        var handlerType = typeof(TestNotificationHandler);
        var exception = new Exception("Test");

        await dlq.AddAsync(notification, handlerType, exception);
        var messages = await dlq.GetMessagesAsync();
        var messageId = messages.First().Id;

        // Act
        await dlq.DeleteAsync(messageId);

        // Assert
        var remainingMessages = await dlq.GetMessagesAsync();
        Assert.Empty(remainingMessages);
    }
}

