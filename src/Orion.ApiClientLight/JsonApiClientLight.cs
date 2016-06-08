using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orion.ApiClientLight.Contents;
using Orion.ApiClientLight.Logger;

namespace Orion.ApiClientLight
{
    public class JsonApiClientLight : IJsonApiClientLight
    {
        private readonly IHttpClientLight _httpClient;

        protected virtual JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings();
        }
        #region Properties

        public Dictionary<string, string> Headers => _httpClient.Headers;

        public RetryPolicy RetryPolicy {
            get { return _httpClient.RetryPolicy; }
            set { _httpClient.RetryPolicy = value; }
        }

        public ILogger Logger {
            get { return _httpClient.Logger; }
            set { _httpClient.Logger = value; }
        }

        #endregion

        #region Constructors
        public JsonApiClientLight() {
            _httpClient = new HttpClientLight();
        }
        public JsonApiClientLight(IHttpClientLight httpClient) {
            _httpClient = httpClient;
        }
        #endregion

        #region Requests
        protected virtual async Task<HttpResponse> RequestAsync(string requestUrl, HttpMethod httpMethod, object playloadData, object urlParameters, CancellationToken token)
        {

            HttpContent requestcontent = null;
            var url = new Url(requestUrl, urlParameters);

            if (IsMethodWithPlayload(httpMethod))
            {
                requestcontent = CreateRequestPlayload(playloadData);
            }
            return await _httpClient.SendRequestAsync(url.ToString(), httpMethod, requestcontent, token);
        }

        protected virtual async Task<HttpResponse<T>> RequestAsync<T>(string requestUrl, HttpMethod httpMethod, object playloadData, object urlParameters, CancellationToken token)
        {
            var response = await RequestAsync(requestUrl, httpMethod, playloadData, urlParameters, token);
            return await response.ToAsync<T>();
        }

        #endregion

        #region Parameters Management
        protected virtual HttpContent CreateRequestPlayload(object data)
        {
            if (data is string)
            {
                return new StringContent((string) data);
            }
            else if (data is HttpContent)
            {
                return (HttpContent) data;
            }
            else if (data is FileInformationContent)
            {
                var fileInformation = (FileInformationContent) data;
                var multipartContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                multipartContent.Add(new StreamContent(fileInformation.Stream), fileInformation.Name, fileInformation.Name);
                return multipartContent;
            }
            return new StringContent(JsonConvert.SerializeObject(data, CreateJsonSerializerSettings()), Encoding.UTF8, "application/json");
        }

        private bool IsMethodWithPlayload(HttpMethod httpMethod)
        {
            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Requests
        public virtual async Task<HttpResponse> GetAsync(string url, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync(url, HttpMethod.Get, null, parameters, token ?? CancellationToken.None);
        }

        public virtual async Task<HttpResponse> PostAsync(string url, object data, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync(url, HttpMethod.Post, data, parameters, token ?? CancellationToken.None);
        }
        public virtual async Task<HttpResponse> PutAsync(string url, object data, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync(url, HttpMethod.Put, data, parameters, token ?? CancellationToken.None);
        }
        public virtual async Task<HttpResponse> DeleteAsync(string url, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync(url, HttpMethod.Delete, null, parameters, token ?? CancellationToken.None);
        }

        #region Typed
        public virtual async Task<HttpResponse<T>> GetAsync<T>(string url, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync<T>(url, HttpMethod.Get, null, parameters, token ?? CancellationToken.None);
        }
        public virtual async Task<HttpResponse<T>> PostAsync<T>(string url, object data, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync<T>(url, HttpMethod.Post, data, parameters, token ?? CancellationToken.None);
        }
        public virtual async Task<HttpResponse<T>> PutAsync<T>(string url, object data, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync<T>(url, HttpMethod.Put, data, parameters, token ?? CancellationToken.None);
        }
        public virtual async Task<HttpResponse<T>> DeleteAsync<T>(string url, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync<T>(url, HttpMethod.Delete, null, parameters, token ?? CancellationToken.None);
        }
        #endregion

        #region Uploads
        public virtual async Task<HttpResponse> UploadFile(string url, Stream file, string filename = null, HttpMethod httpMethod = null, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync(url, httpMethod ?? HttpMethod.Post, new FileInformationContent(file, filename), parameters, token ?? CancellationToken.None);
        }
        public virtual async Task<HttpResponse<T>> UploadFile<T>(string url, Stream file, string filename = null, HttpMethod httpMethod = null, object parameters = null, CancellationToken? token = null)
        {
            return await RequestAsync<T>(url, httpMethod ?? HttpMethod.Post, new FileInformationContent(file, filename), parameters, token ?? CancellationToken.None);
        }
        #endregion
        #endregion
    }
}