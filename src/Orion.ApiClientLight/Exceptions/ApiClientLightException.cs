using System;

namespace Orion.ApiClientLight.Exceptions {
	public class ApiClientLightException : Exception {
		public ApiClientLightException() {
		}

		public ApiClientLightException(string message) : base(message) {
		}

		public ApiClientLightException(string message, Exception exception) : base(message, exception) {
		}
	}
}