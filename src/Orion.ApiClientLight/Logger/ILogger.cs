using System.Net.Http;

namespace Orion.ApiClientLight.Logger
{
    public interface ILogger {
        void LogRequest(string url, HttpMethod httpMethod, HttpContent content);
    }
}
