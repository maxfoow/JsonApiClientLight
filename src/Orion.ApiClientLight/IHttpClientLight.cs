using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Orion.ApiClientLight.Logger;

namespace Orion.ApiClientLight {
    public interface IHttpClientLight {
        Dictionary<string, string> Headers { get; }
        RetryPolicy RetryPolicy { get; set; }
        ILogger Logger { get; set; }

        Task<HttpResponse> SendRequestAsync(string url, HttpMethod httpMethod, HttpContent content,
            CancellationToken token);
    }
}