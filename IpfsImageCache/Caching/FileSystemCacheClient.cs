namespace IpfsImageCache.Caching;

internal class FileSystemCacheClient : ICacheClient
{
    private readonly string _directory;

    public FileSystemCacheClient(IConfiguration configuration)
    {
        _directory = Path.Combine(Directory.GetCurrentDirectory(), configuration["CacheFolder"]!);
    }

    public async Task<byte[]?> GetCachedBytes(string id)
    {
        EnsureDirectoryExists();

        var path = Path.Combine(_directory, id);

        if (File.Exists(path))
        {
            return await File.ReadAllBytesAsync(path);
        }

        return null;
    }

    public async Task CacheBytes(string id, byte[] bytes)
    {
        EnsureDirectoryExists();

        var path = Path.Combine(_directory, id);

        if (File.Exists(path))
        {
            return;
        }

        await File.WriteAllBytesAsync(path, bytes);
    }

    private void EnsureDirectoryExists()
    {
        if (!Directory.Exists(_directory))
        {
            Directory.CreateDirectory(_directory);
        }
    }
}
