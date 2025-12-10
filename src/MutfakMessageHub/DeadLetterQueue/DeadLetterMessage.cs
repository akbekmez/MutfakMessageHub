namespace MutfakMessageHub.DeadLetterQueue;

/// <summary>
/// Represents a message that failed processing and was sent to the dead-letter queue.
/// </summary>
public class DeadLetterMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the dead-letter message.
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
    /// Gets or sets the handler type that failed.
    /// </summary>
    public string HandlerType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stack trace of the error.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the number of processing attempts.
    /// </summary>
    public int Attempts { get; set; } = 1;
}

