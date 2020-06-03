using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.Serialization.Impl
{
    public class SystemJsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(obj, options);
        }

        public async Task<string> SerializeAsync(object obj, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            await using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), options, cancellationToken);

            stream.Position = 0;

            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }

        public TReturn Deserialize<TReturn>(string obj, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<TReturn>(obj, options);
        }

        public async Task<TReturn> DeserializeAsync<TReturn>(string obj, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            await using var stream = new MemoryStream();

            return await JsonSerializer.DeserializeAsync<TReturn>(stream, options, cancellationToken);
        }
    }
}
