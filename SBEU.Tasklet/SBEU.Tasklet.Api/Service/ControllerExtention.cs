﻿using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace SBEU.Tasklet.Api.Service
{
    public class ControllerExt : Controller
    {
        public string UserId
        {
            get
            {
                try
                {
                    var claimIdentity = this.User.Identity as ClaimsIdentity;
                    var userId = claimIdentity.Claims.First(x => x.Type == "Id").Value;
                    return userId;
                }
                catch
                {
                    HttpContext.Abort();
                    return null;
                }
            }
        }
    }
}
