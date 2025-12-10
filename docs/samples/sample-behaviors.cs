using MutfakMessageHub.Pipeline;
using MutfakMessageHub.Abstractions;

public class DemoLoggingBehavior<TReq, TRes>
    : IPipelineBehavior<TReq, TRes>
    where TReq : IRequest<TRes>
{
    public async Task<TRes> Handle(TReq req, CancellationToken cancellationToken, RequestHandlerDelegate<TRes> next)
    {
        Console.WriteLine($"Request: {typeof(TReq).Name}");
        var response = await next();
        Console.WriteLine($"Response: {typeof(TRes).Name}");
        return response;
    }
}
