using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MutfakMessageHub.Configuration;
using MutfakMessageHub.Core;

namespace MutfakMessageHub.DependencyInjection;

/// <summary>
/// Extension methods for setting up MutfakMessageHub services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MutfakMessageHub services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMutfakMessageHub(this IServiceCollection services)
    {
        return services.AddMutfakMessageHub(_ => { });
    }

    /// <summary>
    /// Adds MutfakMessageHub services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configure">An action to configure the <see cref="MutfakMessageHubOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMutfakMessageHub(
        this IServiceCollection services,
        Action<MutfakMessageHubOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var options = new MutfakMessageHubOptions();
        configure(options);

        services.TryAddSingleton(options);
        services.TryAddScoped<IMessageHub, MessageHub>();

        return services;
    }
}

