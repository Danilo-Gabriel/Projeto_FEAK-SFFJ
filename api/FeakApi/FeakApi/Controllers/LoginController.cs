using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeakApi.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        [HttpGet]
        public string Hello()
        {
            return "Olá, mundo..";
        }
    }
}
