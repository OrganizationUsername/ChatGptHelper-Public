using System.Net.Http;
using System.Threading.Tasks;

namespace Helper.Wpf.General;

internal class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    public HttpService() => _httpClient = new HttpClient();
    public Task<byte[]> GetByteArrayAsync(string url) => _httpClient.GetByteArrayAsync(url);
}