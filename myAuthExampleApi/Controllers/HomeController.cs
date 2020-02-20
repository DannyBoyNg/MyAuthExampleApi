using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace myAuthExampleApi.Controllers
{
    [Authorize]
    public class HomeController: ControllerBase
    {
        [Route("/Home")]
        public ActionResult Home()
        {
            var username = User.Identity.Name;
            return Ok($"Welcome {username}! You are now authenticated.");
        }
    }
}
