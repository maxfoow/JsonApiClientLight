using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orion.ApiClientLight.Exceptions;

namespace Orion.ApiClientLight {
	public class HttpResponse {
		private readonly List<Exception> _exceptions;
		public int RetryCount { get; }
		public HttpResponseMessage HttpResponseMessage { get; }

		public HttpResponse(HttpResponseMessage httpResponseMessage, int retryCount, List<Exception> exceptions) {
			_exceptions = exceptions;
			RetryCount = retryCount;
			HttpResponseMessage = httpResponseMessage;
		}

		public async Task<TResponse> FromJsonAsync<TResponse>() {
			try {
				HttpResponseMessage.EnsureSuccessStatusCode();
			}
			catch (Exception e) {
				var content = await HttpResponseMessage.Content.ReadAsStringAsync();
				HttpResponseMessage.Content?.Dispose();
				throw new ApilRequestException("Error during the request. See the inner exception for details.",
					e is ApilRequestException ? e.InnerException : e,
					HttpResponseMessage.StatusCode,
					content);
			}
			using (var stream = await HttpResponseMessage.Content.ReadAsStreamAsync()) {
				using (var sr = new StreamReader(stream)) {
					using (var reader = new JsonTextReader(sr)) {
						try {
							var serializer = new JsonSerializer();
							return serializer.Deserialize<TResponse>(reader);
						}
						catch (Exception ex) {
							throw new ApilJsonException($"Error while processing json deserialization on type {typeof(TResponse).FullName}. See the inner exception for details.", ex,
								reader.ReadAsString());
						}
					}
				}
			}
		}

		internal async Task<HttpResponse<TResponse>> ToAsync<TResponse>() {
			var response = new HttpResponse<TResponse>(HttpResponseMessage, RetryCount, _exceptions);
			await response.InitializeAsync();
			return response;
		}
	}

	public class HttpResponse<TResponse> : HttpResponse {
		public TResponse Response { get; private set; }

		public HttpResponse(HttpResponseMessage httpResponseMessage, int retryCount, List<Exception> exceptions)
			: base(httpResponseMessage, retryCount, exceptions) {
		}

		internal async Task InitializeAsync() {
			Response = await FromJsonAsync<TResponse>();
		}
	}
}