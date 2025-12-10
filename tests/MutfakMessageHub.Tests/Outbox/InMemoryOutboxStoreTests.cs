using MutfakMessageHub.Outbox;
using Xunit;

namespace MutfakMessageHub.Tests.Outbox;

public class InMemoryOutboxStoreTests
{
    [Fact]
    public async Task SaveAsync_WithMessage_SavesMessage()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message = new OutboxMessage
        {
            NotificationType = "TestNotification",
            Payload = "{}"
        };

        // Act
        await store.SaveAsync(message);

        // Assert
        var unprocessed = await store.GetUnprocessedAsync();
        Assert.Single(unprocessed);
        Assert.Equal(message.Id, unprocessed.First().Id);
    }

    [Fact]
    public async Task GetUnprocessedAsync_ReturnsOnlyUnprocessedMessages()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message1 = new OutboxMessage { NotificationType = "Test1", Payload = "{}" };
        var message2 = new OutboxMessage { NotificationType = "Test2", Payload = "{}", IsProcessed = true };

        await store.SaveAsync(message1);
        await store.SaveAsync(message2);

        // Act
        var unprocessed = await store.GetUnprocessedAsync();

        // Assert
        Assert.Single(unprocessed);
        Assert.Equal(message1.Id, unprocessed.First().Id);
    }

    [Fact]
    public async Task MarkAsProcessedAsync_WithValidId_MarksAsProcessed()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message = new OutboxMessage { NotificationType = "Test", Payload = "{}" };
        await store.SaveAsync(message);

        // Act
        await store.MarkAsProcessedAsync(message.Id);

        // Assert
        var unprocessed = await store.GetUnprocessedAsync();
        Assert.Empty(unprocessed);
    }

    [Fact]
    public async Task MarkAsFailedAsync_WithValidId_MarksAsFailed()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message = new OutboxMessage { NotificationType = "Test", Payload = "{}" };
        await store.SaveAsync(message);
        var errorMessage = "Test error";

        // Act
        await store.MarkAsFailedAsync(message.Id, errorMessage);

        // Assert
        var unprocessed = await store.GetUnprocessedAsync();
        var failedMessage = unprocessed.First();
        Assert.Equal(errorMessage, failedMessage.ErrorMessage);
        Assert.Equal(1, failedMessage.Attempts);
    }

    [Fact]
    public async Task DeleteProcessedAsync_DeletesOldProcessedMessages()
    {
        // Arrange
        var store = new InMemoryOutboxStore();
        var message1 = new OutboxMessage
        {
            NotificationType = "Test1",
            Payload = "{}",
            IsProcessed = true,
            ProcessedAt = DateTime.UtcNow.AddDays(-2)
        };
        var message2 = new OutboxMessage
        {
            NotificationType = "Test2",
            Payload = "{}",
            IsProcessed = true,
            ProcessedAt = DateTime.UtcNow.AddHours(-1)
        };

        await store.SaveAsync(message1);
        await store.SaveAsync(message2);

        // Act
        var deleted = await store.DeleteProcessedAsync(DateTime.UtcNow.AddDays(-1));

        // Assert
        Assert.Equal(1, deleted);
    }
}

