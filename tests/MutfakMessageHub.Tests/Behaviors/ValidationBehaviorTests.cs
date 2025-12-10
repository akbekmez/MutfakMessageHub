using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Behaviors;
using MutfakMessageHub.Pipeline;
using Xunit;

namespace MutfakMessageHub.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithValidRequest_CallsNext()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ValidationBehavior<ValidRequest, string>>>();
        var behavior = new ValidationBehavior<ValidRequest, string>(loggerMock.Object);

        var request = new ValidRequest { Name = "Test" };
        var expectedResult = "Success";
        var nextCalled = false;

        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResult);
        };

        // Act
        var result = await behavior.Handle(request, CancellationToken.None, next);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ValidationBehavior<ValidRequest, string>>>();
        var behavior = new ValidationBehavior<ValidRequest, string>(loggerMock.Object);

        var request = new ValidRequest { Name = string.Empty }; // Invalid

        RequestHandlerDelegate<string> next = () => Task.FromResult("Success");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, CancellationToken.None, next));
    }
}

public class ValidRequest : IRequest<string>
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;
}

