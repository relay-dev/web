using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Web.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<string> GetArgument(this HttpRequest request, string argumentName)
        {
            string argumentValue = request.Query[argumentName];

            if (!string.IsNullOrEmpty(argumentValue))
            {
                return argumentValue;
            }

            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            return data?.GetType().GetProperty(argumentName).GetValue(data, null);
        }

        public static async Task<TArgument> GetArgument<TArgument>(this HttpRequest request, string argumentName) where TArgument : class
        {
            return await GetArgument(request, argumentName) as TArgument;
        }
    }
}
