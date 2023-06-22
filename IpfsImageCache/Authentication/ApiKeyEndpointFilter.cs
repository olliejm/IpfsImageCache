namespace IpfsImageCache.Authentication;

internal class ApiKeyEndpointFilter : IEndpointFilter
{
    private readonly string? _apiKey;

    public ApiKeyEndpointFilter(IConfiguration configuration)
    {
        _apiKey = configuration["ApiKey"];
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Query.TryGetValue("k", out var k) || k != _apiKey)
        {
            return TypedResults.Unauthorized();
        }

        return await next(context);
    }
}
