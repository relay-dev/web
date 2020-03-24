using Core.Exceptions;
using Microservices.Exceptions;
using Microservices.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microservices.Filters
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            IJsonSerializer jsonSerializer,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _jsonSerializer = jsonSerializer;
            _logger = loggerFactory.CreateLogger("Default");
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string errorMessage = null;

            var httpStatusCode = HttpStatusCode.InternalServerError;

            if (ex is MicroserviceException serviceException)
            {
                httpStatusCode = GetHttpStatusCode(serviceException.ErrorCode);
                errorMessage = serviceException.Message;
            }
            else if (ex is ValidationException validationException)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
                errorMessage = _jsonSerializer.Serialize(validationException.ValidationFailureResult);
            }

            Log(ex, httpStatusCode, errorMessage);

            var result = new
            {
                error = errorMessage ?? ex.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;

            string serializedResult = _jsonSerializer.Serialize(result);

            return context.Response.WriteAsync(serializedResult);
        }

        private void Log(Exception ex, HttpStatusCode httpStatusCode, string errorMessage)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.GatewayTimeout:
                    _logger.LogWarning("{0} - {1}", httpStatusCode.ToString(), ex.Message);
                    break;
                case HttpStatusCode.BadRequest:
                    _logger.LogWarning("{0} - {1} - {2}", httpStatusCode.ToString(), ex.Message, errorMessage);
                    break;
                default:
                    _logger.LogError(ex, httpStatusCode.ToString());
                    break;
            }
        }

        private HttpStatusCode GetHttpStatusCode(string errorCode)
        {
            if (errorCode == ErrorCode.BADR)
                return HttpStatusCode.BadRequest;
            else if (errorCode == ErrorCode.AUTH)
                return HttpStatusCode.Unauthorized;
            else if (errorCode == ErrorCode.FRBD)
                return HttpStatusCode.Forbidden;
            else if (errorCode == ErrorCode.NTFD)
                return HttpStatusCode.NotFound;
            else if (errorCode == ErrorCode.EROR)
                return HttpStatusCode.InternalServerError;
            else if (errorCode == ErrorCode.NTIM)
                return HttpStatusCode.NotImplemented;
            else if (errorCode == ErrorCode.BDGW)
                return HttpStatusCode.BadGateway;
            else if (errorCode == ErrorCode.SVCU)
                return HttpStatusCode.ServiceUnavailable;
            else if (errorCode == ErrorCode.TIME)
                return HttpStatusCode.GatewayTimeout;
            else
                return HttpStatusCode.InternalServerError;
        }
    }
}
