using System;
using System.Collections.Generic;
using System.Net;

namespace Orion.ApiClientLight.Exceptions {
	public class ApilRequestException : Exception {
		public List<Exception> Exceptions { get; }
		public HttpStatusCode StatusCode { get; }
		public string Content { get; }

		public ApilRequestException(string message, List<Exception> exceptions) : base (message) {
			Exceptions = exceptions;
		}

		public ApilRequestException(string message, Exception exception, HttpStatusCode statusCode, string content) : base(message, exception) {
			StatusCode = statusCode;
			Content = content;
		}
	}
}