using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Attributes;
using MutfakMessageHub.Pipeline;

namespace MutfakMessageHub.Behaviors;

/// <summary>
/// Pipeline behavior that caches request responses.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache? _cache;
    private readonly ILogger<CacheBehavior<TRequest, TResponse>>? _logger;
    private static readonly ConcurrentDictionary<Type, CacheAttribute?> CacheAttributeCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">Optional memory cache. If not provided, caching is disabled.</param>
    /// <param name="logger">Optional logger.</param>
    public CacheBehavior(
        IMemoryCache? cache = null,
        ILogger<CacheBehavior<TRequest, TResponse>>? logger = null)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        // Check if caching is enabled and cache is available
        if (_cache == null)
        {
            return await next();
        }

        // Check if request has Cache attribute
        var cacheAttribute = GetCacheAttribute(typeof(TRequest));
        if (cacheAttribute == null)
        {
            return await next();
        }

        // Generate cache key
        var cacheKey = GenerateCacheKey(request, cacheAttribute);

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse) && cachedResponse != null)
        {
            _logger?.LogDebug("Cache hit for request of type {RequestType} with key {CacheKey}", typeof(TRequest).Name, cacheKey);
            return cachedResponse;
        }

        // Execute handler and cache result
        _logger?.LogDebug("Cache miss for request of type {RequestType} with key {CacheKey}", typeof(TRequest).Name, cacheKey);
        var response = await next();

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheAttribute.DurationSeconds)
        };

        _cache.Set(cacheKey, response, cacheOptions);

        return response;
    }

    private static CacheAttribute? GetCacheAttribute(Type requestType)
    {
        return CacheAttributeCache.GetOrAdd(requestType, type =>
        {
            return type.GetCustomAttributes(typeof(CacheAttribute), true)
                .FirstOrDefault() as CacheAttribute;
        });
    }

    private static string GenerateCacheKey(TRequest request, CacheAttribute attribute)
    {
        var prefix = attribute.KeyPrefix ?? typeof(TRequest).Name;
        
        // Serialize request to create a unique key
        // In a production scenario, you might want to use a more efficient serialization
        try
        {
            var requestJson = JsonSerializer.Serialize(request);
            return $"{prefix}:{requestJson.GetHashCode()}";
        }
        catch
        {
            // Fallback to type name + hash code if serialization fails
            return $"{prefix}:{request.GetHashCode()}";
        }
    }
}

