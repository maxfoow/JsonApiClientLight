using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Orion.ApiClientLight.Exceptions
{
    public class RequestException : ApiClientLightException
    {
        public RequestException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public RequestException(string message) : base(message)
        {
        }
    }

    public class RequestCanceledException : RequestException
    {
        public RequestCanceledException(TaskCanceledException exception)
            : base("Request has been canceled", exception)
        {
        }
    }

    public class RequestUnknownException : RequestException
    {
        public RequestUnknownException(Exception exception) 
            : base("Unknown error happened. See inner for more.", exception)
        {
        }
    }

    public class RequestHttpException : RequestException
    {
        public IReadOnlyCollection<Exception> Exceptions { get; }

        public RequestHttpException(IReadOnlyCollection<Exception> exceptions) 
            : base("Request failed. See inner for more.")
        {
            Exceptions = exceptions;
        }
    }

    public class RequestResponseException : ApiClientLightException
    {
        public HttpResponse HttpResponse { get; }
        public HttpStatusCode HttpStatusCode => HttpResponse.StatusCode;

        public RequestResponseException(HttpResponse httpResponse)
        {
            HttpResponse = httpResponse;
        }
    }
}