using Core.Exceptions;
using Core.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Web.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            IJsonSerializer jsonSerializer,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _jsonSerializer = jsonSerializer;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string errorMessage = null;

            var httpStatusCode = HttpStatusCode.InternalServerError;

            if (ex is CoreException serviceException)
            {
                httpStatusCode = GetHttpStatusCode(serviceException.ErrorCode);
                errorMessage = serviceException.Message;
            }
            else if (ex is ValidationException validationException)
            {
                httpStatusCode = HttpStatusCode.BadRequest;
                errorMessage = validationException.Message;
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

            if (errorCode == ErrorCode.AUTH)
                return HttpStatusCode.Unauthorized;

            if (errorCode == ErrorCode.FRBD)
                return HttpStatusCode.Forbidden;

            if (errorCode == ErrorCode.NTFD)
                return HttpStatusCode.NotFound;

            if (errorCode == ErrorCode.EROR)
                return HttpStatusCode.InternalServerError;

            if (errorCode == ErrorCode.NTIM)
                return HttpStatusCode.NotImplemented;

            if (errorCode == ErrorCode.BDGW)
                return HttpStatusCode.BadGateway;

            if (errorCode == ErrorCode.SVCU)
                return HttpStatusCode.ServiceUnavailable;

            if (errorCode == ErrorCode.TIME)
                return HttpStatusCode.GatewayTimeout;

            return HttpStatusCode.InternalServerError;
        }
    }
}
