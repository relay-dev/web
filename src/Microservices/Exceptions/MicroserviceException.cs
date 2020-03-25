using Core.Exceptions;
using System;
using System.Net;

namespace Microservices.Exceptions
{
    public class MicroserviceException : CoreException
    {
        public HttpStatusCode HttpStatusCode { get; protected set; }

        public MicroserviceException(string message)
            : base(message)
        {
            HttpStatusCode = HttpStatusCode.InternalServerError;
        }

        public MicroserviceException(Exception innerException, string message)
            : base(innerException, message)
        {
            HttpStatusCode = HttpStatusCode.InternalServerError;
        }

        public MicroserviceException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            ErrorCode = GetErrorCode(httpStatusCode);
        }

        public MicroserviceException(Exception innerException, HttpStatusCode httpStatusCode, string message)
            : base(innerException, message)
        {
            HttpStatusCode = httpStatusCode;
            ErrorCode = GetErrorCode(httpStatusCode);
        }

        private string GetErrorCode(HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.BadRequest:
                    return Core.Exceptions.ErrorCode.BADR;
                case HttpStatusCode.Unauthorized:
                    return Core.Exceptions.ErrorCode.AUTH;
                case HttpStatusCode.Forbidden:
                    return Core.Exceptions.ErrorCode.FRBD;
                case HttpStatusCode.NotFound:
                    return Core.Exceptions.ErrorCode.NTFD;
                case HttpStatusCode.InternalServerError:
                    return Core.Exceptions.ErrorCode.EROR;
                case HttpStatusCode.NotImplemented:
                    return Core.Exceptions.ErrorCode.NTIM;
                case HttpStatusCode.BadGateway:
                    return Core.Exceptions.ErrorCode.BDGW;
                case HttpStatusCode.ServiceUnavailable:
                    return Core.Exceptions.ErrorCode.SVCU;
                case HttpStatusCode.GatewayTimeout:
                    return Core.Exceptions.ErrorCode.TIME;
                default:
                    return Core.Exceptions.ErrorCode.UNKN;
            }
        }
    }
}
