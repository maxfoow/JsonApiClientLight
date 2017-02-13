using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.ApiClientLight.Demo.Controllers;

namespace Orion.ApiClientLight.Demo.Demo
{
    public class SimpleGet
    {
        public async Task<IReadOnlyDictionary<string, object>> Start(string url)
        {
            var jsonApiClient = new JsonApiClient(url);
            var results = new Dictionary<string, object>();
            results.Add("Simple Get", await SimpleGetRequestAsync(jsonApiClient));
            results.Add("Timeout", await TimeoutRequestAsync());
            //results.Add("Bad Address", await BadAddressRequestAsync());
            //results.Add("BadRequest", await BadRequestAsync(jsonApiClient));
            //results.Add("NotFound", await NotFoundAsync(jsonApiClient));
            //results.Add("InternalError", await InternalErrorAsync(jsonApiClient));
            return results;
        }

        private async Task<object> SimpleGetRequestAsync(IJsonApiClientLight jsonApiClient)
        {
            return await jsonApiClient.GetAsync<TestController.SimpleGetResponse>("Test/SimpleGet");
        }

        private async Task<object> TimeoutRequestAsync()
        {
            var jsonApiClient = new JsonApiClient("http://www.google.com:81");
            try
            {
                return await jsonApiClient.GetAsync("Test");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private async Task<object> BadAddressRequestAsync()
        {
            var jsonApiClient = new JsonApiClient("http://255.255.255.255");
            try
            {
                return await jsonApiClient.GetAsync<TestController.SimpleGetResponse>("Test/SimpleGet");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private async Task<object> BadRequestAsync(IJsonApiClientLight jsonApiClient)
        {
            try
            {
                return await jsonApiClient.GetAsync("Test/BadRequestError");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private async Task<object> NotFoundAsync(IJsonApiClientLight jsonApiClient)
        {
            try
            {
                return await jsonApiClient.GetAsync("Test/NotFound");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private async Task<object> InternalErrorAsync(IJsonApiClientLight jsonApiClient)
        {
            try
            {
                return await jsonApiClient.GetAsync("Test/InternalError");
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}