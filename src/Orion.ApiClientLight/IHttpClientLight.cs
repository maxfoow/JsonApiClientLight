using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Orion.ApiClientLight {
    public interface IHttpClientLight {
        Dictionary<string, string> Headers { get; }
        RetryPolicy RetryPolicy { get; set; }

        Task<HttpResponse> SendRequestAsync(string url,
            HttpMethod httpMethod,
            HttpContent httpContent,
            CancellationToken cancellationToken,
            bool throwIfNotSuccessStatusCode = false);
    }
}