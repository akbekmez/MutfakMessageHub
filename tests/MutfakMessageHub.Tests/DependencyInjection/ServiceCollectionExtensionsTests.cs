using Microsoft.Extensions.DependencyInjection;
using MutfakMessageHub.Configuration;
using MutfakMessageHub.Core;
using MutfakMessageHub.DeadLetterQueue;
using MutfakMessageHub.DependencyInjection;
using MutfakMessageHub.Outbox;
using MutfakMessageHub.Pipeline;
using Xunit;

namespace MutfakMessageHub.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMutfakMessageHub_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMutfakMessageHub();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<IMessageHub>());
        Assert.NotNull(serviceProvider.GetService<MutfakMessageHubOptions>());
    }

    [Fact]
    public void AddMutfakMessageHub_WithConfiguration_AppliesConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMutfakMessageHub(o =>
        {
            o.EnableCaching();
            o.EnableRetry();
            o.EnableOutbox();
            o.EnableTelemetry();
            o.EnableDeadLetterQueue();
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<MutfakMessageHubOptions>();
        Assert.True(options.CachingEnabled);
        Assert.True(options.RetryEnabled);
        Assert.True(options.OutboxEnabled);
        Assert.True(options.TelemetryEnabled);
        Assert.True(options.DeadLetterQueueEnabled);
    }

    [Fact]
    public void AddMutfakMessageHub_WithOutboxEnabled_RegistersOutboxServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMutfakMessageHub(o => o.EnableOutbox());

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<IOutboxStore>());
        Assert.NotNull(serviceProvider.GetService<IOutboxWriter>());
    }

    [Fact]
    public void AddMutfakMessageHub_WithDeadLetterQueueEnabled_RegistersDlqService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMutfakMessageHub(o => o.EnableDeadLetterQueue());

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<IDeadLetterQueue>());
    }

    [Fact]
    public void AddMutfakMessageHub_WithBehaviorsEnabled_RegistersBehaviors()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMutfakMessageHub(o =>
        {
            o.EnableCaching();
            o.EnableRetry();
            o.EnableTelemetry();
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        // Behaviors are registered as open generics, so we can't directly resolve them
        // Instead, verify that the services are registered by checking the service collection
        var behaviorDescriptors = services.Where(s => 
            s.ServiceType.IsGenericType && 
            s.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
        Assert.NotEmpty(behaviorDescriptors);
    }

    [Fact]
    public void AddMutfakMessageHub_WithNullServices_ThrowsArgumentNullException()
    {
        // Act & Assert
        IServiceCollection? nullServices = null;
        Assert.Throws<ArgumentNullException>(() =>
            nullServices!.AddMutfakMessageHub());
    }

    [Fact]
    public void AddMutfakMessageHub_WithNullConfigure_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            services.AddMutfakMessageHub(null!));
    }
}

