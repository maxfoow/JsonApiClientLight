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
using Orion.ApiClientLight.Contents;

namespace Orion.ApiClientLight {
	public class JsonApiClientLight : BaseApiClientLight, IJsonApiClientLight {
		protected virtual JsonSerializerSettings CreateJsonSerializerSettings() {
			return new JsonSerializerSettings();
		}

		#region Requests
		protected virtual async Task<HttpResponse> RequestAsync(string url, HttpMethod httpMethod, object data, CancellationToken token) {

			HttpContent requestcontent = null;

			if (IsMethodWithPlayload(httpMethod)) {
				requestcontent = CreateRequestPlayload(data);
			}
			else {
				url += CreateUrlParameters(data);
			}
			return await SendRequestAsync(url, httpMethod, requestcontent, token);
		}

		protected virtual async Task<HttpResponse<T>> RequestAsync<T>(string url, HttpMethod httpMethod, object data, CancellationToken token) {
			var response = await RequestAsync(url, httpMethod, data, token);
			return await response.ToAsync<T>();
		}

		#endregion

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
		public virtual async Task<HttpResponse> GetAsync(string url, object parameters, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Get, parameters, token ?? CancellationToken.None);
		}

		public virtual async Task<HttpResponse> PostAsync(string url, object data, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Post, data, token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponse> PutAsync(string url, object data, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Put, data, token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponse> DeleteAsync(string url, object data, CancellationToken? token = null) {
			return await RequestAsync(url, HttpMethod.Delete, data, token ?? CancellationToken.None);
		}

		#region Typed
		public virtual async Task<HttpResponse<T>> GetAsync<T>(string url, object parameters, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Get, parameters, token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponse<T>> PostAsync<T>(string url, object data, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Post, data, token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponse<T>> PutAsync<T>(string url, object data, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Put, data, token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponse<T>> DeleteAsync<T>(string url, object data, CancellationToken? token = null) {
			return await RequestAsync<T>(url, HttpMethod.Delete, data, token ?? CancellationToken.None);
		}
		#endregion

		#region Uploads
		public virtual async Task<HttpResponse> UploadFile(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null) {
			return await RequestAsync(url, httpMethod ?? HttpMethod.Post, new FileInformationContent(file, filename), token ?? CancellationToken.None);
		}
		public virtual async Task<HttpResponse<T>> UploadFile<T>(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null) {
			return await RequestAsync<T>(url, httpMethod ?? HttpMethod.Post, new FileInformationContent(file, filename), token ?? CancellationToken.None);
		}
		#endregion
		#endregion
	}
}