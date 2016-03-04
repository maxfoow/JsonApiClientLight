using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orion.ApiClientLight.Exceptions;

namespace Orion.ApiClientLight {
	public abstract class BaseApiClientLight {
		public const string HeaderContentType = "Content-Type";
		public const string HeaderContentTypeJson = "application/json";
		private readonly HttpClient _httpClient;
		private RetryPolicy _retryPolicy;

		#region Properties
		public Dictionary<string, string> Headers { get; }

		public RetryPolicy RetryPolicy {
			get { return _retryPolicy; }
			set {
				_retryPolicy = value ?? RetryPolicy.NoRetry;
			}
		}
		#endregion

		#region Constructors
		protected BaseApiClientLight() {
			Headers = new Dictionary<string, string> { { HeaderContentType, HeaderContentTypeJson } };
		}

		protected BaseApiClientLight(HttpClient httpClient) {
			_httpClient = httpClient;
			Headers = new Dictionary<string, string>();
		}

		#endregion

		protected async Task<HttpResponse> SendRequestAsync(string url, HttpMethod httpMethod, HttpContent content,
			CancellationToken token) {
			var actualRetryCount = 0;
			var exceptions = new List<Exception>();
			do {
				if (actualRetryCount != 0)
					await Task.Delay(RetryPolicy.Next(actualRetryCount), token);
				try {
					token.ThrowIfCancellationRequested();

					var client = _httpClient ?? CreateHttpClient();
					var requestMessage = new HttpRequestMessage {
						Method = httpMethod,
						RequestUri = new Uri(url),
						Content = content
					};
					var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, token);
					content?.Dispose();
					if (response.StatusCode != HttpStatusCode.Accepted) {
						throw new ErrorRequestException(new HttpResponse(response, actualRetryCount, exceptions));
					}
					return new HttpResponse(response, actualRetryCount, exceptions);
				}
				catch (TaskCanceledException) {
					throw;
				}
				catch (Exception e) {
					exceptions.Add(e);
				}
				++actualRetryCount;
			} while (actualRetryCount <= RetryPolicy.RetryCount);
			throw new RequestException("Error during the request. See the inner exception for details.", exceptions);
		}

		protected virtual HttpClient CreateHttpClient() {
			var httpClient = new HttpClient();
			foreach (var header in Headers) {
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
			}
			return httpClient;
		}
	}
}