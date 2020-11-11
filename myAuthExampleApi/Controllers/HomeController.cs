using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace myAuthExampleApi.Controllers
{
    [ApiController]
    [Authorize]
    public class HomeController: ControllerBase
    {
        [Route("/Home")]
        [HttpGet]
        public ActionResult Home()
        {
            var username = User?.Identity?.Name;
            return Ok($"Welcome {username}! You are now authenticated.");
        }
    }
}
