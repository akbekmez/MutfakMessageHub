using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Configuration;
using MutfakMessageHub.Core;
using Xunit;

namespace MutfakMessageHub.Tests.Core;

public class MessageHubTests
{
    [Fact]
    public async Task Send_WithValidRequest_ReturnsResponse()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(new MutfakMessageHubOptions());
        services.AddScoped<IMessageHub, MessageHub>();

        var handler = new TestRequestHandler();
        services.AddScoped<IRequestHandler<TestRequest, string>>(_ => handler);

        var serviceProvider = services.BuildServiceProvider();
        var messageHub = serviceProvider.GetRequiredService<IMessageHub>();

        var request = new TestRequest { Message = "Hello" };

        // Act
        var result = await messageHub.Send(request);

        // Assert
        Assert.Equal("Hello", result);
        Assert.True(handler.Handled);
    }

    [Fact]
    public async Task Send_WithNoHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(new MutfakMessageHubOptions());
        services.AddScoped<IMessageHub, MessageHub>();

        var serviceProvider = services.BuildServiceProvider();
        var messageHub = serviceProvider.GetRequiredService<IMessageHub>();

        var request = new TestRequest { Message = "Hello" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => messageHub.Send(request));
    }

    [Fact]
    public async Task Publish_WithValidNotification_CallsAllHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(new MutfakMessageHubOptions());
        services.AddScoped<IMessageHub, MessageHub>();

        var handler1 = new TestNotificationHandler();
        var handler2 = new TestNotificationHandler();
        services.AddScoped<INotificationHandler<TestNotification>>(_ => handler1);
        services.AddScoped<INotificationHandler<TestNotification>>(_ => handler2);

        var serviceProvider = services.BuildServiceProvider();
        var messageHub = serviceProvider.GetRequiredService<IMessageHub>();

        var notification = new TestNotification { Message = "Test" };

        // Act
        await messageHub.Publish(notification);

        // Assert
        Assert.True(handler1.Handled);
        Assert.True(handler2.Handled);
    }

    [Fact]
    public async Task PublishParallel_WithValidNotification_CallsAllHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(new MutfakMessageHubOptions());
        services.AddScoped<IMessageHub, MessageHub>();

        var handler1 = new TestNotificationHandler();
        var handler2 = new TestNotificationHandler();
        services.AddScoped<INotificationHandler<TestNotification>>(_ => handler1);
        services.AddScoped<INotificationHandler<TestNotification>>(_ => handler2);

        var serviceProvider = services.BuildServiceProvider();
        var messageHub = serviceProvider.GetRequiredService<IMessageHub>();

        var notification = new TestNotification { Message = "Test" };

        // Act
        await messageHub.PublishParallel(notification);

        // Assert
        Assert.True(handler1.Handled);
        Assert.True(handler2.Handled);
    }
}

// Test helpers
public class TestRequest : IRequest<string>
{
    public string Message { get; set; } = string.Empty;
}

public class TestRequestHandler : IRequestHandler<TestRequest, string>
{
    public bool Handled { get; private set; }

    public Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        Handled = true;
        return Task.FromResult(request.Message);
    }
}

public class TestNotification : INotification
{
    public string Message { get; set; } = string.Empty;
}

public class TestNotificationHandler : INotificationHandler<TestNotification>
{
    public bool Handled { get; private set; }

    public Task Handle(TestNotification notification, CancellationToken cancellationToken)
    {
        Handled = true;
        return Task.CompletedTask;
    }
}

