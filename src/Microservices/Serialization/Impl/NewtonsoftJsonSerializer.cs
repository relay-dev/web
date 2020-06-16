using Newtonsoft.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
            return Task.Run(() => Serialize(obj, options), cancellationToken);
        }

        public TReturn Deserialize<TReturn>(string obj, JsonSerializerOptions options = null)
        {
            return JsonConvert.DeserializeObject<TReturn>(obj, ToJsonSerializerSettings(options));
        }

        public Task<TReturn> DeserializeAsync<TReturn>(string obj, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Deserialize<TReturn>(obj, options), cancellationToken);
        }

        /// <summary>
        /// Gets a new instance of NewtonsoftJsonSerializer
        /// </summary>
        /// <remarks>
        /// This is here because the DistributedCacheHelper can't use SystemJsonSerializer, so it's new's up an instance of NewtonsoftJsonSerializer
        /// Since the constructors can change, let consumers use this method to do it. When the constructor changes the fix is only here and no consumers will break
        /// </remarks>
        public static NewtonsoftJsonSerializer New => new NewtonsoftJsonSerializer();

        private JsonSerializerSettings ToJsonSerializerSettings(JsonSerializerOptions options)
        {
            if (options == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = options.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include
            };

            if (options.MaxDepth > 0)
            {
                settings.MaxDepth = options.MaxDepth;
            }

            return settings;
        }
    }
}
