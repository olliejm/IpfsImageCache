namespace IpfsImageCache.Ipfs;

internal class IpfsClient
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public IpfsClient(
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    internal Task<Stream> GetStream(string id)
    {
        return _httpClient.GetStreamAsync(string.Format(_configuration["IpfsHostFormat"]!, id));
    }
}
