using System.Threading.Tasks;
using Moq;
using System.Net.Http;
using System.Threading;
using Orion.ApiClientLight;
using Xunit;

namespace Orion.ApiClientLight_PCL.Tests
{
    public class JsonApiClientLightTests
    {
        private const string StandardUrl = "http://exemple.com";
        private readonly JsonApiClientLight _jsonApiClientLight;
        private readonly Mock<IHttpClientLight> _httpClientLightMock;
        private readonly object _parameters = new { id = 1 };
        private readonly object _playload = new { id = 1 };

        public JsonApiClientLightTests()
        {
            _httpClientLightMock = new Mock<IHttpClientLight>();
            _jsonApiClientLight = new JsonApiClientLight(_httpClientLightMock.Object);
        }

        [Fact]
        public void WhenRetryPolicyIsSetToNull_ExpectTheRetryPolicyIsDisabled()
        {
            _jsonApiClientLight.RetryPolicy = null;

            Assert.True(_jsonApiClientLight.RetryPolicy.IsDisabled());
        }

        [Fact]
        public async Task GivenAnUrl_WhenGetAsync_ThenShouldCallSendRequestAsyncWithGoodParameter()
        {

            var result = await _jsonApiClientLight.GetAsync(StandardUrl);

            _httpClientLightMock.Verify(h => h.SendRequestAsync(StandardUrl, HttpMethod.Get, null, CancellationToken.None));
        }

        [Fact]
        public async Task GivenAnUrlAndParameters_WhenGetAsync_ThenShouldCallSendRequestAsyncWithGoodParameter()
        {
            var result = await _jsonApiClientLight.GetAsync(StandardUrl, _parameters);

            _httpClientLightMock.Verify(h => h.SendRequestAsync(StandardUrl + "?id=1", HttpMethod.Get, null, CancellationToken.None));
        }

        [Fact]
        public async Task GivenAnUrlWithPlayLoad_WhenPostAsync_ThenShouldCallSendRequestAsyncWithGoodParameter()
        {
            var result = await _jsonApiClientLight.PostAsync(StandardUrl, _playload);

            _httpClientLightMock.Verify(h => h.SendRequestAsync(StandardUrl, HttpMethod.Post, It.IsAny<StringContent>(), CancellationToken.None));
        }

        [Fact]
        public async Task GivenAnUrlWithPlayLoadAndParameters_WhenPostAsync_ThenShouldCallSendRequestAsyncWithGoodParameter()
        {
            var result = await _jsonApiClientLight.PostAsync(StandardUrl, _playload, _parameters);

            _httpClientLightMock.Verify(h => h.SendRequestAsync(StandardUrl + "?id=1", HttpMethod.Post, It.IsAny<StringContent>(), CancellationToken.None));
        }

        [Fact]
        public async Task GivenAnUrlWithOnlyParameters_WhenPostAsync_ThenShouldCallSendRequestAsyncWithGoodParameter()
        {
            var result = await _jsonApiClientLight.PostAsync(StandardUrl, null, _parameters);

            _httpClientLightMock.Verify(h => h.SendRequestAsync(StandardUrl + "?id=1", HttpMethod.Post, It.IsAny<StringContent>(), CancellationToken.None));
        }
    }
}
