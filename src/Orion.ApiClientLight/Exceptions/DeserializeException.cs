using System;

namespace Orion.ApiClientLight.Exceptions {
	public class DeserializeException : ApiClientLightException {
		public string Json { get; }

		public DeserializeException(string message, Exception exception, string json) : base(message, exception) {
			Json = json;
		}
	}
}