using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Attributes;
using MutfakMessageHub.Sample.Common.DTOs;

namespace MutfakMessageHub.Sample.Features.Products;

[Cache(DurationSeconds = 60)] // Cache for 60 seconds
public class GetProductsQuery : IRequest<List<ProductDto>>
{
}

