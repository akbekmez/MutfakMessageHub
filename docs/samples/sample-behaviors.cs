public class DemoLoggingBehavior<TReq, TRes>
    : IPipelineBehavior<TReq, TRes>
{
    public async Task<TRes> Handle(TReq req, CancellationToken token, RequestHandlerDelegate<TRes> next)
    {
        Console.WriteLine($"Request: {typeof(TReq).Name}");
        var response = await next();
        Console.WriteLine($"Response: {typeof(TRes).Name}");
        return response;
    }
}
