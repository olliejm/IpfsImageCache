using IpfsImageCache.Api;
using IpfsImageCache.Authentication;
using IpfsImageCache.Caching;
using IpfsImageCache.Events;
using IpfsImageCache.Ipfs;
using IpfsImageCache.Logging;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
using Serilog;
using Serilog.Events;

const string TransportName = "transport";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Async(wt => wt.Console())
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var logtailToken = builder.Configuration.GetValue<string>("LogtailToken");

    builder.Host.UseSerilog((ctx, lc) =>
    {
        if (builder.Environment.IsProduction() && !string.IsNullOrWhiteSpace(logtailToken))
        {
            lc.WriteTo.LogtailSink(logtailToken);
        }

        lc.ReadFrom.Configuration(ctx.Configuration);
    });

    builder.Services.AddSingleton<ICacheClient, FileSystemCacheClient>();
    builder.Services.AddHttpClient<IpfsClient>();

    builder.Services.AddRebus(conf =>
    {
        return conf
            .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), TransportName))
            .Logging(l => l.Serilog(Log.Logger))
            .Routing(r => r.TypeBased()
                .Map<CacheEvent>(TransportName));
    });

    builder.Services.AutoRegisterHandlersFromAssemblyOf<CacheEvent>();

    var app = builder.Build();

    app.UseHttpsRedirection();

    app
        .MapGet("/{id}", Endpoints.Get)
        .AddEndpointFilter<ApiKeyEndpointFilter>();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
