using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microservices.Serialization.Impl
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj, JsonSerializerOptions options = null)
        {
            return JsonConvert.SerializeObject(obj, ToJsonSerializerSettings(options));
        }

        public Task<string> SerializeAsync(object obj, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public TReturn Deserialize<TReturn>(string obj, JsonSerializerOptions options = null)
        {
            return JsonConvert.DeserializeObject<TReturn>(obj, ToJsonSerializerSettings(options));
        }

        public Task<TReturn> DeserializeAsync<TReturn>(string obj, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        private JsonSerializerSettings ToJsonSerializerSettings(JsonSerializerOptions options)
        {
            return new JsonSerializerSettings
            {

            };
        }
    }
}
