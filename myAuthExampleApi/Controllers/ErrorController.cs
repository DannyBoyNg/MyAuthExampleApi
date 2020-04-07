using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ng.Services;
using Services.UserServ;
using System;
using System.Globalization;

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
            if (e is CooldownException) Response.Headers.Add("cooldownLeft", (e as CooldownException).CooldownLeft.Value.TotalSeconds.ToString("N0", CultureInfo.InvariantCulture));
            var msg = GetBaseExceptionMessage(e);
            return BadRequest(msg);
        }
    }
}
