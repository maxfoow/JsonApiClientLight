
using System;
using System.Dynamic;
using Orion.ApiClientLight;
using Xunit;

namespace Orion.ApiClientLight_PCL.Tests {
    public class UrlTests {
        private const string StandardUrl = "http://exemple.com";
        private const string StringParameter = "?parameter=1";

        [Fact]
        public void GivenUrlWithOutParameter_WhenToString_ThenShouldReturnTheSameUrl() {
            var url = new Url(StandardUrl);

            var result = url.ToString();

            Assert.Equal(StandardUrl, result);
        }

        [Fact]
        public void GivenUrlWithStringParameter_WhenToString_ThenShouldJustAppendUrlAndParameter() {
            var url = new Url(StandardUrl, StringParameter);

            var result = url.ToString();

            Assert.Equal(StandardUrl + StringParameter, result);
        }

        [Fact]
        public void GivenUrlWithObjectParameter_WhenToString_ThenShouldCreateAParameterForEachProperty() {
            var url = new Url(StandardUrl, new { id = 1, name = "test" });

            var result = url.ToString();

            Assert.Equal(StandardUrl + "?id=1&name=test", result);
        }

        [Fact]
        public void GivenUrlWithObjectWhoContainsObject_WhenToString_ThenShouldCreateAParameterWithJsonInsideValue() {
            var url = new Url(StandardUrl, new { prop = new { id = 1, name = "test"} });

            var result = url.ToString();

            Assert.Equal(StandardUrl + "?prop={\"id\":1,\"name\":\"test\"}", result);
        }

        [Fact]
        public void GivenUrlWithExpandoObject_WhenToString_ThenShouldCreateAParameterForEachProperty() {
            dynamic exp = new ExpandoObject();
            exp.id = 1;
            exp.name = "test";
            var url = new Url(StandardUrl, exp);

            var result = url.ToString();

            Assert.Equal(StandardUrl + "?id=1&name=test", result);

        }

        [Fact]
        public void GivenUrlWithExpandoObjectWhoContainsObject_WhenToString_ThenShouldCreateAParameterWithJsonInsideValue()
        {
            dynamic exp = new ExpandoObject();
            exp.prop = new {id = 1, name = "test"};
            var url = new Url(StandardUrl, exp);

            var result = url.ToString();

            Assert.Equal(StandardUrl + "?prop={\"id\":1,\"name\":\"test\"}", result);
        }

        [Fact]
        public void WhenConstructUrlWithNullArgument_ThenShouldThrowAnException() {
            Assert.Throws<ArgumentNullException>(() => {
                var url = new Url(null);
            });
        }
    }
}