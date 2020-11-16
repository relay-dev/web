using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;

namespace Web.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetArgument(this HttpRequest request, string argumentName)
        {
            string argumentValue = request.Query[argumentName];

            if (!string.IsNullOrEmpty(argumentValue))
            {
                return argumentValue;
            }

            string requestBody = new StreamReader(request.Body).ReadToEnd();

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            return data?.GetType().GetProperty(argumentName).GetValue(data, null);
        }

        public static TRequestBody GetBody<TRequestBody>(this HttpRequest request)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();

            return JsonConvert.DeserializeObject<TRequestBody>(requestBody);
        }

        public static TArgument GetArgument<TArgument>(this HttpRequest request, string argumentName) where TArgument : class
        {
            return GetArgument(request, argumentName) as TArgument;
        }
    }
}
