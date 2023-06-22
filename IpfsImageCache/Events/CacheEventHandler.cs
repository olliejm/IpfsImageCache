using IpfsImageCache.Caching;
using Rebus.Handlers;

namespace IpfsImageCache.Events;

internal sealed class CacheEventHandler : IHandleMessages<CacheEvent>
{
    private readonly ICacheClient _cacheClient;

    public CacheEventHandler(ICacheClient cacheClient)
    {
        _cacheClient = cacheClient;
    }

    public async Task Handle(CacheEvent message)
    {
        await _cacheClient.CacheBytes(message.Id, message.Data);
    }
}
