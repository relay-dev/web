using Newtonsoft.Json;

namespace Microservices.Serialization.Impl
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
