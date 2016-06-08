using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Orion.ApiClientLight.Logger;

namespace Orion.ApiClientLight {
	public interface IJsonApiClientLight {
		Dictionary<string, string> Headers { get; }
		RetryPolicy RetryPolicy { get; set; }
        ILogger Logger { get; set; }

		/// <summary>
		/// Make HTTP Get request asynchronous
		/// </summary>
		/// <param name="url">url of target</param>
		/// <param name="parameters">query string parameters</param>
		/// <param name="token">cancellation token</param>
		/// <returns>HttpResponseMessage of request</returns>
		Task<HttpResponse> GetAsync(string url, object parameters = null, CancellationToken? token = null);

        /// <summary>
        /// Make HTTP POST request asynchronous
        /// </summary>
        /// <param name="url">url of target</param>
        /// <param name="data">data for the request's content</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="token">cancellation token</param>
        /// <returns>HttpResponseMessage of request</returns>
        Task<HttpResponse> PostAsync(string url, object data, object parameters = null, CancellationToken? token = null);

        /// <summary>
        /// Make HTTP PUT request asynchronous
        /// </summary>
        /// <param name="url">url of target</param>
        /// <param name="data">data for the request's content</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="token">cancellation token</param>
        /// <returns>HttpResponseMessage of request</returns>
        Task<HttpResponse> PutAsync(string url, object data, object parameters = null, CancellationToken? token = null);

        /// <summary>
        /// Make HTTP DELETE request asynchronous
        /// </summary>
        /// <param name="url">url of target</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="token">cancellation token</param>
        /// <returns>HttpResponseMessage of request</returns>
        Task<HttpResponse> DeleteAsync(string url, object parameters = null, CancellationToken? token = null);

		/// <summary>
		/// Make HTTP GET request asynchronous with automatic deserialization of response.
		/// </summary>
		/// <typeparam name="T">Deserialization type</typeparam>
		/// <param name="url">url of target</param>
		/// <param name="parameters">query string parameters</param>
		/// <param name="token">cancellation token</param>
		/// <returns>Instance of T filled with response content</returns>
		Task<HttpResponse<T>> GetAsync<T>(string url, object parameters = null, CancellationToken? token = null);

        /// <summary>
        /// Make HTTP POST request asynchronous with automatic deserialization of response.
        /// </summary>
        /// <typeparam name="T">Deserialization type</typeparam>
        /// <param name="url">url of target</param>
        /// <param name="data">data for the request's content</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Instance of T filled with response content</returns>
        Task<HttpResponse<T>> PostAsync<T>(string url, object data, object parameters = null, CancellationToken? token = null);

        /// <summary>
        /// Make HTTP PUT request asynchronous with automatic deserialization of response.
        /// </summary>
        /// <typeparam name="T">Deserialization type</typeparam>
        /// <param name="url">url of target</param>
        /// <param name="data">data for the request's content</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Instance of T filled with response content</returns>
        Task<HttpResponse<T>> PutAsync<T>(string url, object data, object parameters = null, CancellationToken? token = null);

		/// <summary>
		/// Make HTTP DELETE request asynchronous with automatic deserialization of response.
		/// </summary>
		/// <typeparam name="T">Deserialization type</typeparam>
		/// <param name="url">url of target</param>
		/// <param name="parameters">query string parameters</param>
		/// <param name="token">cancellation token</param>
		/// <returns>Instance of T filled with response content</returns>
		Task<HttpResponse<T>> DeleteAsync<T>(string url, object parameters = null, CancellationToken? token = null);

        /// <summary>
        /// Make HTTP Request, default POST, for upload a file.
        /// </summary>
        /// <param name="url">url of target</param>
        /// <param name="file">file's stream content</param>
        /// <param name="filename">the name of file, default "file"</param>
        /// <param name="httpMethod">Request's httpMethod</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="token">cancellation token</param>
        /// <returns>HttpResponseMessage of target</returns>
        Task<HttpResponse> UploadFile(string url, Stream file, string filename = null, HttpMethod httpMethod = null, object parameters = null, CancellationToken? token = null);

        /// <summary>
        /// Make HTTP Request, default POST, for upload a file with automatic deserialization.
        /// </summary>
        /// <typeparam name="T">Deserialization type</typeparam>
        /// <param name="url">url of target</param>
        /// <param name="file">file's stream content</param>
        /// <param name="filename">the name of file, default "file"</param>
        /// <param name="httpMethod">Request's httpMethod</param>
        /// <param name="parameters">query string parameters</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Instance of T filled with response content</returns>
        Task<HttpResponse<T>> UploadFile<T>(string url, Stream file, string filename = null, HttpMethod httpMethod = null, object parameters = null, CancellationToken? token = null);
	}
}