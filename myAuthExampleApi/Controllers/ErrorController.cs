using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services.UserService;
using System;

namespace myAuthExampleApi.Controllers
{
    //Global Error Handler
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            //local function
            static string GetBaseExceptionMessage(Exception e)
            {
                while (e?.InnerException != null) e = e.InnerException;
                return e.Message;
            }
            //Logic
            var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var e = context.Error;
            Request.Path = context.Path;
            if (e is NotAuthorizedException || e is NotAuthenticatedException) return Unauthorized();
            var msg = GetBaseExceptionMessage(e);
            return BadRequest(msg);
        }
    }
}
