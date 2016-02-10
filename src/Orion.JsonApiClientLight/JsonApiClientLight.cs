using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orion.ApiLight.Exceptions;
using Orion.JsonApiClientLight.Contents;

namespace Orion.JsonApiClientLight {
	public class JsonApiClientLight : IJsonApiClientLight {
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
		public JsonApiClientLight() {
			Headers = new Dictionary<string, string>();
		}
		#endregion

		protected virtual HttpClient CreateHttpClient() {
			var httpClient = new HttpClient();
			foreach (var header in Headers) {
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
			}
			return httpClient;
		}

		protected virtual JsonSerializerSettings CreateJsonSerializerSettings() {
			return new JsonSerializerSettings();
		}

		#region Requests
		protected virtual async Task<HttpResponseMessage> RequestAsync(string url, HttpMethod httpMethod,
			 object data, CancellationToken token) {

			HttpContent requestcontent = null;

			if (IsMethodWithPlayload(httpMethod)) {
				requestcontent = CreateRequestPlayload(data);
			}
			else {
				url += CreateUrlParameters(data);
			}
			var actualRetryCount = 0;
			Exception lastException;
			do {
				if (actualRetryCount != 0)
					await Task.Delay(RetryPolicy.Next(actualRetryCount), token);
				try {
					token.ThrowIfCancellationRequested();

					var client = CreateHttpClient();
					var requestMessage = new HttpRequestMessage {
						Method = httpMethod,
						RequestUri = new Uri(url),
						Content = requestcontent
					};
					var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, token);
					requestcontent?.Dispose();
					return response;
				}
				catch (TaskCanceledException) {
					throw;
				}
				catch (Exception e) {
					lastException = e;
				}
				++actualRetryCount;
			} while (actualRetryCount <= RetryPolicy.RetryCount);
			throw new ApilRequestException("Error during the request. See the inner exception for details.", lastException);
		}

		protected virtual async Task<T> RequestAsync<T>(string url, HttpMethod httpMethod,
			 object data, CancellationToken token) {
			var response = await RequestAsync(url, httpMethod, data, token);
			return await DeserializeHttpResponse<T>(token, response);
		}

		#endregion

		protected virtual async Task<T> DeserializeHttpResponse<T>(CancellationToken token, HttpResponseMessage response) {
			try {
				response.EnsureSuccessStatusCode();
			}
			catch (Exception e) {
				var content = await response.Content.ReadAsStringAsync();
				response.Content?.Dispose();
				throw new ApilRequestException("Error during the request. See the inner exception for details.",
					e is ApilRequestException ? e.InnerException : e,
					response.StatusCode,
					content);
			}
			token.ThrowIfCancellationRequested();
			using (var stream = await response.Content.ReadAsStreamAsync()) {
				token.ThrowIfCancellationRequested();

				using (var sr = new StreamReader(stream)) {
					using (var reader = new JsonTextReader(sr)) {
						try {
							var serializer = new JsonSerializer();
							return serializer.Deserialize<T>(reader);
						}
						catch (Exception ex) {
							throw new ApilJsonException("Error while processing json. See the inner exception for details.", ex,
								reader.ReadAsString());
						}
					}
				}
			}
		}

		#region Parameters Management
		protected virtual string CreateUrlParameters(object data) {
			var parameters = new Dictionary<string, string>();
			var type = data.GetType();
			foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties) {
				var value = propertyInfo.GetValue(data);
				if (value.GetType().GetTypeInfo().IsPrimitive)
					parameters.Add(propertyInfo.Name, value.ToString());
				else
					parameters.Add(propertyInfo.Name, JsonConvert.SerializeObject(value, CreateJsonSerializerSettings()));
			}
			var result = new StringBuilder("?");
			var count = 0;
			foreach (var parameter in parameters) {
				if (count > 0)
					result.Append("&");
				result.Append($"{parameter.Key}={parameter.Value}");
				++count;
			}
			return result.ToString();
		}

		protected virtual HttpContent CreateRequestPlayload(object data) {
			if (data is string) {
				return new StringContent((string) data);
			}
			else if (data is FileInformationContent) {
				var fileInformation = (FileInformationContent) data;
				var multipartContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
				multipartContent.Add(new StreamContent(fileInformation.Stream), fileInformation.Name, fileInformation.Name);
				return multipartContent;
			}
			return new StringContent(JsonConvert.SerializeObject(data, CreateJsonSerializerSettings()), Encoding.UTF8, "application/json");
		}

		private bool IsMethodWithPlayload(HttpMethod httpMethod) {
			if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put) {
				return true;
			}
			return false;
		}
		#endregion

		#region Requests
		public virtual async Task<HttpResponseMessage> GetAsync(string url, object parameters, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Get, parameters, token ?? CancellationToken.None);
		}

		public virtual async Task<HttpResponseMessage> PostAsync(string url, object data, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Post, data, token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponseMessage> PutAsync(string url, object data, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Put, data, token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponseMessage> DeleteAsync(string url, object data, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Delete, data, token ?? CancellationToken.None);
		}

		#region Typed
		public virtual async Task<T> GetAsync<T>(string url, object parameters, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Get, parameters, token ?? CancellationToken.None);
		}
		public virtual async Task<T> PostAsync<T>(string url, object data, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Post, data, token ?? CancellationToken.None);
		}
		public virtual async Task<T> PutAsync<T>(string url, object data, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Put, data, token ?? CancellationToken.None);
		}
		public virtual async Task<T> DeleteAsync<T>(string url, object data, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Delete, data, token ?? CancellationToken.None);
		}
		#endregion

		#region Uploads
		public virtual async Task<HttpResponseMessage> UploadFile(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null) {
			return await RequestAsync(url, httpMethod ?? HttpMethod.Post, new FileInformationContent(file, filename), token ?? CancellationToken.None);
		}
		public virtual async Task<T> UploadFile<T>(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null) {
			return await RequestAsync<T>(url, httpMethod ?? HttpMethod.Post, new FileInformationContent(file, filename), token ?? CancellationToken.None);
		}
		#endregion
		#endregion
	}
}