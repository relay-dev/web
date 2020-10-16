using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net.Http;

namespace Web.Rest.Filters
{
    public class UsernameHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!IsWriteOperation(context))
            {
                return;
            }

            var openApiParameter = new OpenApiParameter
            {
                Name = "x-username",
                In = ParameterLocation.Header,
                Required = true,
                Example = new OpenApiString("customer@email.com")
            };

            operation.Parameters.Add(openApiParameter);
        }

        private static bool IsWriteOperation(OperationFilterContext context)
        {
            return string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Delete.Method, StringComparison.InvariantCultureIgnoreCase)
                   || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Patch.Method, StringComparison.InvariantCultureIgnoreCase)
                   || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Post.Method, StringComparison.InvariantCultureIgnoreCase)
                   || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Put.Method, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
