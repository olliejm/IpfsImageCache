using IpfsImageCache.Caching;
using IpfsImageCache.Events;
using IpfsImageCache.Ipfs;
using Microsoft.AspNetCore.Mvc;
using NetVips;
using Rebus.Bus;

namespace IpfsImageCache.Api
{
    internal static class Endpoints
    {
        public async static Task<IResult> Get(
            string id,
            [FromServices] ICacheClient cacheClient,
            [FromServices] IpfsClient ipfsClient,
            [FromServices] IBus bus)
        {
            var cached = await cacheClient.GetCachedBytes(id);

            if (cached is not null)
            {
                return Results.File(cached, "image/png");
            }

            using var stream = await ipfsClient.GetStream(id);
            var bytes = Image
                .NewFromStream(stream)
                .ThumbnailImage(400)
                .PngsaveBuffer();

            await bus.Send(new CacheEvent
            {
                Id = id,
                Data = bytes
            });

            return Results.File(bytes, "image/png");
        }
    }
}
