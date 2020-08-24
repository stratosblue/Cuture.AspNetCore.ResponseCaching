using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Test.WebHost.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [ResponseCaching(3600, Mode = CacheMode.PathUniqueness)]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get1()
        {
            return "Get1";
        }

        [HttpGet]
        public string Get2()
        {
            return "Get2";
        }

        [HttpGet]
        public string Get3()
        {
            return "Get3";
        }

        [HttpGet]
        public string Get4()
        {
            return "Get4";
        }

        [HttpGet]
        public string Get5()
        {
            return "Get5";
        }
    }
}