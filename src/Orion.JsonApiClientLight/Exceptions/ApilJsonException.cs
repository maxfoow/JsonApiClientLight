using System;

namespace Orion.ApiLight.Exceptions {
	public class ApilJsonException : Exception {
		public string Json { get; }

		public ApilJsonException(string message, Exception exception, string json) : base(message, exception) {
			Json = json;
		}
	}
}