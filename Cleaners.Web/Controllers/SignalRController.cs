﻿using Microsoft.AspNetCore.Mvc;

namespace Cleaners.Web.Controllers
{
    [Route("signal-r")]
    public class SignalRController : AdminControllerBase
    {
        public IActionResult Index() => View();
    }
}