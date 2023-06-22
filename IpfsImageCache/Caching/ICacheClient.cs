namespace IpfsImageCache.Caching;

internal interface ICacheClient
{
    Task<byte[]?> GetCachedBytes(string id);

    Task CacheBytes(string id, byte[] bytes);
}
