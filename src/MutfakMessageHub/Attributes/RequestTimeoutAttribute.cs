namespace MutfakMessageHub.Attributes;

/// <summary>
/// Attribute to specify a timeout for request processing.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequestTimeoutAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the timeout in milliseconds.
    /// </summary>
    public int Milliseconds { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestTimeoutAttribute"/> class.
    /// </summary>
    /// <param name="milliseconds">The timeout in milliseconds.</param>
    public RequestTimeoutAttribute(int milliseconds)
    {
        Milliseconds = milliseconds;
    }
}

