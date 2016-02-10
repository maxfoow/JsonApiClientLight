using System;
using System.Net;

namespace Orion.ApiLight.Exceptions {
	public class ApilRequestException : Exception {
		public HttpStatusCode StatusCode { get; }
		public string Content { get; }

		public ApilRequestException(string message, Exception exception) : base (message, exception) {
		}

		public ApilRequestException(string message, Exception exception, HttpStatusCode statusCode, string content) : base(message, exception) {
			StatusCode = statusCode;
			Content = content;
		}
	}
}