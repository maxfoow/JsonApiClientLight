using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orion.ApiClientLight.Exceptions;

namespace Orion.ApiClientLight {
    public class HttpResponse {
        private readonly List<Exception> _exceptions;

        public HttpResponse(HttpResponseMessage httpResponseMessage, int retryCount, List<Exception> exceptions) {
            _exceptions = exceptions;
            RetryCount = retryCount;
            HttpResponseMessage = httpResponseMessage;
        }

        public int RetryCount { get; }
        private HttpResponseMessage HttpResponseMessage { get; }

        public HttpContent Response => HttpResponseMessage.Content;
        public bool IsSuccessStatusCode => HttpResponseMessage.IsSuccessStatusCode;
        public HttpStatusCode StatusCode => HttpResponseMessage.StatusCode;

        public async Task<TResponse> FromJsonAsync<TResponse>() {
            try {
                HttpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception) {
                throw new RequestResponseException(this);
            }
            using (var stream = await HttpResponseMessage.Content.ReadAsStreamAsync()) {
                using (var sr = new StreamReader(stream)) {
                    using (var reader = new JsonTextReader(sr)) {
                        try {
                            var serializer = new JsonSerializer();
                            return serializer.Deserialize<TResponse>(reader);
                        }
                        catch (Exception ex) {
                            throw new DeserializeException(
                                $"Error while processing json deserialization on type {typeof(TResponse).FullName}. See the inner exception for details.",
                                ex,
                                reader.ReadAsString());
                        }
                    }
                }
            }
        }

        public TResponse FromJson<TResponse>() {
            var task = FromJsonAsync<TResponse>();
            try {
                task.Wait();

            }
            catch (AggregateException e) {
                throw e.InnerException;
            }
            return task.Result;
        }

        internal async Task<HttpResponse<TResponse>> ToAsync<TResponse>() {
            var response = new HttpResponse<TResponse>(HttpResponseMessage, RetryCount, _exceptions);
            await response.InitializeAsync();
            return response;
        }
    }

    public class HttpResponse<TResponse> : HttpResponse {
        public HttpResponse(HttpResponseMessage httpResponseMessage, int retryCount, List<Exception> exceptions)
            : base(httpResponseMessage, retryCount, exceptions) {
        }

        public new TResponse Response { get; private set; }

        internal async Task InitializeAsync() {
            Response = await FromJsonAsync<TResponse>();
        }
    }
}