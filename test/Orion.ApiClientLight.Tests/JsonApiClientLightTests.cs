using Xunit;

namespace Orion.ApiClientLight.Tests
{
    public class JsonApiClientLightTests
    {
	    private readonly JsonApiClientLight _jsonApiClientLight;

	    public JsonApiClientLightTests() {
		    _jsonApiClientLight = new JsonApiClientLight();
	    }

		[Fact]
	    public void WhenRetryPolicyIsSetToNull_ExpectTheRetryPolicyIsDisabled() {
		    _jsonApiClientLight.RetryPolicy = null;

			Assert.True(_jsonApiClientLight.RetryPolicy.IsDisabled());
	    }
    }
}
