using System;
using Microsoft.AspNetCore.Mvc;

namespace Orion.ApiClientLight.Demo.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet("SimpleGet")]
        public SimpleGetResponse SimpleGet()
        {
            return new SimpleGetResponse();
        }

        [HttpGet("BadRequestError")]
        public BadRequestResult BadRequestError()
        {
            return this.BadRequest();
        }

        [HttpGet("InternalError")]
        public void InternalError()
        {
            throw new NullReferenceException();
        }

        public class SimpleGetResponse
        {
            public string Value = "Ok";
        }
    }
}