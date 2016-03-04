using System;
using System.Collections.Generic;
using System.Net;

namespace Orion.ApiClientLight.Exceptions {
	public class RequestException : ApiClientLightException {
		public List<Exception> Exceptions { get; }
		public HttpStatusCode StatusCode { get; }
		public string Content { get; }

		public RequestException(string message, List<Exception> exceptions) : base(message) {
			Exceptions = exceptions;
		}

		public RequestException(string message, Exception exception, HttpStatusCode statusCode, string content) : base(message, exception) {
			StatusCode = statusCode;
			Content = content;
		}
	}
}