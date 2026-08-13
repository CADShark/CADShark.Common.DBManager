using System;
using System.Net;

namespace OpenManage.Client.Http
{
    public enum OpenManageErrorKind
    {
        Http,
        Api,
        Network,
        InvalidResponse
    }

    public sealed class OpenManageApiException : Exception
    {
        public OpenManageApiException(
            string message,
            OpenManageErrorKind errorKind,
            HttpStatusCode? statusCode = null,
            string errorCode = null,
            string responseBody = null,
            Exception innerException = null)
            : base(message, innerException)
        {
            ErrorKind = errorKind;
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ResponseBody = responseBody;
        }

        public OpenManageErrorKind ErrorKind { get; }

        public HttpStatusCode? StatusCode { get; }

        public string ErrorCode { get; }

        public string ResponseBody { get; }
    }
}
