namespace IpfsImageCache.Events;

internal sealed record CacheEvent
{
    public string Id { get; init; } = null!;

    public byte[] Data { get; init; } = null!;
}
