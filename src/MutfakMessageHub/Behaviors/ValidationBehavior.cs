using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Behaviors;

/// <summary>
/// Pipeline behavior that validates request objects using data annotations.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">Optional logger.</param>
    public ValidationBehavior(ILogger<ValidationBehavior<TRequest, TResponse>>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);

        if (!isValid)
        {
            var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
            _logger?.LogWarning("Validation failed for request of type {RequestType}: {Errors}", typeof(TRequest).Name, errors);
            throw new ValidationException($"Validation failed for request of type {typeof(TRequest).Name}: {errors}");
        }

        return await next();
    }
}

