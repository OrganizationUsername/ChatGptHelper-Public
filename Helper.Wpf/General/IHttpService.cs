using System.Threading.Tasks;

namespace Helper.Wpf.General;

public interface IHttpService
{
    Task<byte[]> GetByteArrayAsync(string url);
}