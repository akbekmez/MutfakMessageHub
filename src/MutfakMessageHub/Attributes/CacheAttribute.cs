namespace MutfakMessageHub.Attributes;

/// <summary>
/// Attribute to mark requests that should be cached.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class CacheAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the cache duration in seconds.
    /// </summary>
    public int DurationSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the cache key prefix. If not specified, the request type name is used.
    /// </summary>
    public string? KeyPrefix { get; set; }
}

