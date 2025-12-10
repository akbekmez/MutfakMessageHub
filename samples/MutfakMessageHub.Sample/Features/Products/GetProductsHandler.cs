using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Sample.Common.DTOs;

namespace MutfakMessageHub.Sample.Features.Products;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private static readonly List<ProductDto> _products = new()
    {
        new ProductDto { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10 },
        new ProductDto { Id = 2, Name = "Mouse", Price = 29.99m, Stock = 50 },
        new ProductDto { Id = 3, Name = "Keyboard", Price = 79.99m, Stock = 30 },
        new ProductDto { Id = 4, Name = "Monitor", Price = 299.99m, Stock = 15 }
    };

    public Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // Simulate database query delay
        return Task.FromResult(_products);
    }
}

