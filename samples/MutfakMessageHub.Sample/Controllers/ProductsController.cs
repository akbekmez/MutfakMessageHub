using Microsoft.AspNetCore.Mvc;
using MutfakMessageHub.Core;
using MutfakMessageHub.Sample.Features.Products;

namespace MutfakMessageHub.Sample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMessageHub _messageHub;

    public ProductsController(IMessageHub messageHub)
    {
        _messageHub = messageHub;
    }

    /// <summary>
    /// Get all products (cached for 60 seconds)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetProducts()
    {
        var products = await _messageHub.Send(new GetProductsQuery());
        return Ok(products);
    }

    /// <summary>
    /// Test timeout behavior (will timeout after 2 seconds)
    /// </summary>
    [HttpGet("slow")]
    public async Task<ActionResult> SlowQuery()
    {
        try
        {
            var result = await _messageHub.Send(new SlowQuery());
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(408, "Request timeout");
        }
    }
}

