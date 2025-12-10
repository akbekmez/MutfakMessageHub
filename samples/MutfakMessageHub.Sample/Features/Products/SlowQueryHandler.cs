using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.Sample.Features.Products;

public class SlowQueryHandler : IRequestHandler<SlowQuery, string>
{
    public async Task<string> Handle(SlowQuery request, CancellationToken cancellationToken)
    {
        // Simulate slow operation (3 seconds - will timeout)
        await Task.Delay(3000, cancellationToken);
        return "This should timeout";
    }
}

