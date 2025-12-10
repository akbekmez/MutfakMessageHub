namespace MutfakMessageHub.Configuration;

/// <summary>
/// Configuration options for MutfakMessageHub.
/// </summary>
public class MutfakMessageHubOptions
{
    /// <summary>
    /// Gets or sets whether caching is enabled.
    /// </summary>
    public bool CachingEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether retry is enabled.
    /// </summary>
    public bool RetryEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether outbox pattern is enabled.
    /// </summary>
    public bool OutboxEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether telemetry is enabled.
    /// </summary>
    public bool TelemetryEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether dead-letter queue is enabled.
    /// </summary>
    public bool DeadLetterQueueEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether notifications should be published in parallel by default.
    /// </summary>
    public bool PublishParallelByDefault { get; set; }

    /// <summary>
    /// Enables caching behavior.
    /// </summary>
    /// <returns>The options instance for chaining.</returns>
    public MutfakMessageHubOptions EnableCaching()
    {
        CachingEnabled = true;
        return this;
    }

    /// <summary>
    /// Enables retry behavior.
    /// </summary>
    /// <returns>The options instance for chaining.</returns>
    public MutfakMessageHubOptions EnableRetry()
    {
        RetryEnabled = true;
        return this;
    }

    /// <summary>
    /// Enables outbox pattern.
    /// </summary>
    /// <returns>The options instance for chaining.</returns>
    public MutfakMessageHubOptions EnableOutbox()
    {
        OutboxEnabled = true;
        return this;
    }

    /// <summary>
    /// Enables telemetry.
    /// </summary>
    /// <returns>The options instance for chaining.</returns>
    public MutfakMessageHubOptions EnableTelemetry()
    {
        TelemetryEnabled = true;
        return this;
    }

    /// <summary>
    /// Enables dead-letter queue.
    /// </summary>
    /// <returns>The options instance for chaining.</returns>
    public MutfakMessageHubOptions EnableDeadLetterQueue()
    {
        DeadLetterQueueEnabled = true;
        return this;
    }
}

