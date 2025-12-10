namespace MutfakMessageHub.Outbox;

/// <summary>
/// Represents a message stored in the outbox for reliable delivery.
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the outbox message.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the type name of the notification.
    /// </summary>
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full type name of the notification.
    /// </summary>
    public string NotificationTypeFullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized notification payload.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the processing timestamp (null if not processed yet).
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the message has been processed.
    /// </summary>
    public bool IsProcessed { get; set; }

    /// <summary>
    /// Gets or sets the number of processing attempts.
    /// </summary>
    public int Attempts { get; set; }

    /// <summary>
    /// Gets or sets the error message if processing failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

