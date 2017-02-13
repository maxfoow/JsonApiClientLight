using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orion.ApiClientLight.Exceptions;
using Orion.Extensions.Logging;

namespace Orion.ApiClientLight {
    public class HttpClientLight : IHttpClientLight {
        public const string HeaderContentType = "Content-Type";
        public const string HeaderContentTypeJson = "application/json";
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private RetryPolicy _retryPolicy;

        public HttpClientLight() {
            Headers = new Dictionary<string, string> {{HeaderContentType, HeaderContentTypeJson}};
            RetryPolicy = null;
        }

        public HttpClientLight(ILoggerFactory loggerFactory) {
            Headers = new Dictionary<string, string> {{HeaderContentType, HeaderContentTypeJson}};
            RetryPolicy = null;
            _logger = loggerFactory != null ? (ILogger) loggerFactory.CreateLogger<HttpClientLight>() : new LoggerMock();
        }

        public HttpClientLight(HttpClient httpClient = null, ILoggerFactory loggerFactory = null) {
            _httpClient = httpClient;
            _logger = loggerFactory != null ? (ILogger) loggerFactory.CreateLogger<HttpClientLight>() : new LoggerMock();
            Headers = new Dictionary<string, string>();
            RetryPolicy = null;
        }

        public Dictionary<string, string> Headers { get; }

        public RetryPolicy RetryPolicy {
            get { return _retryPolicy; }
            set { _retryPolicy = value ?? RetryPolicy.NoRetry; }
        }


        public async Task<HttpResponse> SendRequestAsync(string url,
            HttpMethod httpMethod,
            HttpContent httpContent,
            CancellationToken cancellationToken,
            bool throwIfNotSuccessStatusCode = false) {
            var actualRetryCount = 0;
            var exceptions = new List<Exception>();
            using (_logger?.BeginScope($"[{httpMethod}] {url}")) {
                do {
                    if (actualRetryCount != 0)
                        await Task.Delay(RetryPolicy.Next(actualRetryCount), cancellationToken);
                    var timer = new TimerHelper();
                    try {
                        cancellationToken.ThrowIfCancellationRequested();

                        var client = _httpClient ?? CreateHttpClient();
                        var requestMessage = new HttpRequestMessage {
                            Method = httpMethod,
                            RequestUri = new Uri(url),
                            Content = httpContent
                        };
                        var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead,
                            cancellationToken);
                        timer.Stop();
                        httpContent?.Dispose();
                        var httpResponse = new HttpResponse(response, actualRetryCount, exceptions);
                        _logger.LogHttpRequest(url, httpMethod.ToString(), timer.StartTime, timer.Duration,
                            responseCode: ((int) response.StatusCode).ToString());
                        if (throwIfNotSuccessStatusCode && !response.IsSuccessStatusCode)
                            throw new RequestResponseException(httpResponse);
                        return httpResponse;
                    }
                    catch (HttpRequestException e) {
                        timer.Stop();
                        _logger.LogHttpRequest(url, httpMethod.ToString(), timer.StartTime, timer.Duration, e);
                        exceptions.Add(e);
                        ++actualRetryCount;
                    }
                    catch (TaskCanceledException e) {
                        timer.Stop();
                        _logger.LogHttpRequest(url, httpMethod.ToString(), timer.StartTime, timer.Duration, e);
                        throw new RequestCanceledException(e);
                    }
                    catch (RequestResponseException) {
                        throw;
                    }
                    catch (Exception e) {
                        timer.Stop();
                        _logger.LogHttpRequest(url, httpMethod.ToString(), timer.StartTime, timer.Duration, e);
                        throw new RequestUnknownException(e);
                    }
                } while (actualRetryCount <= RetryPolicy.RetryCount);
                throw new RequestHttpException(exceptions);
            }
        }

        protected virtual HttpClient CreateHttpClient() {
            var httpClient = new HttpClient();
            foreach (var header in Headers)
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            return httpClient;
        }

        private class LoggerMock : ILogger {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter) {
            }

            public bool IsEnabled(LogLevel logLevel) {
                return false;
            }

            public IDisposable BeginScope<TState>(TState state) {
                return new NullDisposable();
            }

            private class NullDisposable : IDisposable {
                public void Dispose() {
                }
            }
        }
    }
}