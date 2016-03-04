namespace Orion.ApiClientLight.Exceptions {
	public class ErrorRequestException : ApiClientLightException {
		public HttpResponse HttpResponse { get; }

		public ErrorRequestException(HttpResponse httpResponse) {
			HttpResponse = httpResponse;
		}
	}
}