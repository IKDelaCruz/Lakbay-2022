using Infrastructure.Services.WebAuthentication;
using Project.Models;
using Project.Models.API.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        public static AuthUserModel UserIdentity { get { return WebFormsAuthenticationService.Identity; } }

        public static int UserId => UserIdentity?.Id ?? 0;
        public static string GUID => UserIdentity?.Guid ?? "";
    }
}