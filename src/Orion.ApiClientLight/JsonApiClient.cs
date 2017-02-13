using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Orion.ApiClientLight {
    public class JsonApiClient : JsonApiClientLight, IJsonApiClient {
        public JsonApiClient() {
        }

        public JsonApiClient(string baseUrl, ILoggerFactory loggerFactory = null)
            : base(loggerFactory) {
            BaseUrl = baseUrl;
        }

        public JsonApiClient(string baseUrl, IHttpClientLight httpClient = null, ILoggerFactory loggerFactory = null)
            : base(httpClient, loggerFactory) {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }

        public override Task<HttpResponse> GetAsync(string url, object parameters = null,
            CancellationToken? token = null) {
            return base.GetAsync(BaseUrl + url, parameters, token);
        }

        public override Task<HttpResponse<T>> GetAsync<T>(string url, object parameters = null,
            CancellationToken? token = null) {
            return base.GetAsync<T>(BaseUrl + url, parameters, token);
        }

        public override Task<HttpResponse> PostAsync(string url, object data, object parameters = null,
            CancellationToken? token = null) {
            return base.PostAsync(BaseUrl + url, data, parameters, token);
        }

        public override Task<HttpResponse<T>> PostAsync<T>(string url, object data, object parameters = null,
            CancellationToken? token = null) {
            return base.PostAsync<T>(BaseUrl + url, data, parameters, token);
        }

        public override Task<HttpResponse> PutAsync(string url, object data, object parameters = null,
            CancellationToken? token = null) {
            return base.PutAsync(BaseUrl + url, data, parameters, token);
        }

        public override Task<HttpResponse<T>> PutAsync<T>(string url, object data, object parameters = null,
            CancellationToken? token = null) {
            return base.PutAsync<T>(BaseUrl + url, data, parameters, token);
        }

        public override Task<HttpResponse> DeleteAsync(string url, object parameters = null,
            CancellationToken? token = null) {
            return base.DeleteAsync(BaseUrl + url, parameters, token);
        }

        public override Task<HttpResponse<T>> DeleteAsync<T>(string url, object parameters = null,
            CancellationToken? token = null) {
            return base.DeleteAsync<T>(BaseUrl + url, parameters, token);
        }

        public override Task<HttpResponse> UploadFile(string url, Stream file, string filename = null,
            HttpMethod httpMethod = null,
            object parameters = null, CancellationToken? token = null) {
            return base.UploadFile(BaseUrl + url, file, filename, httpMethod, parameters, token);
        }

        public override Task<HttpResponse<T>> UploadFile<T>(string url, Stream file, string filename = null,
            HttpMethod httpMethod = null,
            object parameters = null, CancellationToken? token = null) {
            return base.UploadFile<T>(BaseUrl + url, file, filename, httpMethod, parameters, token);
        }
    }
}