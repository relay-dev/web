using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object obj, JsonSerializerOptions options = null);
        Task<string> SerializeAsync(object obj, JsonSerializerOptions options = null, CancellationToken cancellationToken = default);
        TReturn Deserialize<TReturn>(string obj, JsonSerializerOptions options = null);
        Task<TReturn> DeserializeAsync<TReturn>(string obj, JsonSerializerOptions options = null, CancellationToken cancellationToken = default);
    }
}
