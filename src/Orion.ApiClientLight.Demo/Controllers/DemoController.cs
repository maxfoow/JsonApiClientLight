using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orion.ApiClientLight.Demo.Demo;

namespace Orion.ApiClientLight.Demo.Controllers
{
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        private const string url = "http://localhost:1544/api/";

        // GET api/values
        [HttpGet]
        public async Task<IReadOnlyDictionary<string, object>> Get()
        {
            var results = await new SimpleGet().Start(url);
            return results;
        }
    }
}