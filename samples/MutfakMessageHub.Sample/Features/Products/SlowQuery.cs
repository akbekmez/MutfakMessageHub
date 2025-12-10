using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Attributes;
using MutfakMessageHub.Sample.Common.DTOs;

namespace MutfakMessageHub.Sample.Features.Products;

[RequestTimeout(2000)] // 2 second timeout
public class SlowQuery : IRequest<string>
{
}

