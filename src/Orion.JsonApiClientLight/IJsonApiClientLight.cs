using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Orion.JsonApiClientLight {
	public interface IJsonApiClientLight {
		Dictionary<string, string> Headers { get; }
		RetryPolicy RetryPolicy { get; set; }
		/// <summary>
		/// Make HTTP Get request asynchronous
		/// </summary>
		/// <param name="url">url of target</param>
		/// <param name="parameters">query string parameters</param>
		/// <param name="token">cancellation token</param>
		/// <returns>HttpResponseMessage of request</returns>
		Task<HttpResponseMessage> GetAsync(string url, object parameters, CancellationToken? token = null);
		/// <summary>
		/// Make HTTP POST request asynchronous
		/// </summary>
		/// <param name="url">url of target</param>
		/// <param name="data">data for the request's content</param>
		/// <param name="token">cancellation token</param>
		/// <returns>HttpResponseMessage of request</returns>
		Task<HttpResponseMessage> PostAsync(string url, object data, CancellationToken? token = null);
		/// <summary>
		/// Make HTTP PUT request asynchronous
		/// </summary>
		/// <param name="url">url of target</param>
		/// <param name="data">data for the request's content</param>
		/// <param name="token">cancellation token</param>
		/// <returns>HttpResponseMessage of request</returns>
		Task<HttpResponseMessage> PutAsync(string url, object data, CancellationToken? token = null);
		/// <summary>
		/// Make HTTP DELETE request asynchronous
		/// </summary>
		/// <param name="url">url of target</param>
		/// <param name="parameters">query string parameters</param>
		/// <param name="token">cancellation token</param>
		/// <returns>HttpResponseMessage of request</returns>
		Task<HttpResponseMessage> DeleteAsync(string url, object parameters, CancellationToken? token = null);

		/// <summary>
		/// Make HTTP GET request asynchronous with automatic deserialization of response.
		/// </summary>
		/// <typeparam name="T">Deserialization type</typeparam>
		/// <param name="url">url of target</param>
		/// <param name="parameters">query string parameters</param>
		/// <param name="token">cancellation token</param>
		/// <returns>Instance of T filled with response content</returns>
		Task<T> GetAsync<T>(string url, object parameters, CancellationToken? token = null);
		/// <summary>
		/// Make HTTP POST request asynchronous with automatic deserialization of response.
		/// </summary>
		/// <typeparam name="T">Deserialization type</typeparam>
		/// <param name="url">url of target</param>
		/// <param name="data">data for the request's content</param>
		/// <param name="token">cancellation token</param>
		/// <returns>Instance of T filled with response content</returns>
		Task<T> PostAsync<T>(string url, object data, CancellationToken? token = null);
		/// <summary>
		/// Make HTTP PUT request asynchronous with automatic deserialization of response.
		/// </summary>
		/// <typeparam name="T">Deserialization type</typeparam>
		/// <param name="url">url of target</param>
		/// <param name="data">data for the request's content</param>
		/// <param name="token">cancellation token</param>
		/// <returns>Instance of T filled with response content</returns>
		Task<T> PutAsync<T>(string url, object data, CancellationToken? token = null);
		/// <summary>
		/// Make HTTP DELETE request asynchronous with automatic deserialization of response.
		/// </summary>
		/// <typeparam name="T">Deserialization type</typeparam>
		/// <param name="url">url of target</param>
		/// <param name="parameters">query string parameters</param>
		/// <param name="token">cancellation token</param>
		/// <returns>Instance of T filled with response content</returns>
		Task<T> DeleteAsync<T>(string url, object parameters, CancellationToken? token = null);

		/// <summary>
		/// Make HTTP Request, default POST, for upload a file.
		/// </summary>
		/// <param name="url">url of target</param>
		/// <param name="file">file's stream content</param>
		/// <param name="filename">the name of file, default "file"</param>
		/// <param name="httpMethod">Request's httpMethod</param>
		/// <param name="token">cancellation token</param>
		/// <returns>HttpResponseMessage of target</returns>
		Task<HttpResponseMessage> UploadFile(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null);
		/// <summary>
		/// Make HTTP Request, default POST, for upload a file with automatic deserialization.
		/// </summary>
 		/// <typeparam name="T">Deserialization type</typeparam>
		/// <param name="url">url of target</param>
		/// <param name="file">file's stream content</param>
		/// <param name="filename">the name of file, default "file"</param>
		/// <param name="httpMethod">Request's httpMethod</param>
		/// <param name="token">cancellation token</param>
		/// <returns>Instance of T filled with response content</returns>
		Task<T> UploadFile<T>(string url, Stream file, string filename = null, HttpMethod httpMethod = null, CancellationToken? token = null);
	}
}