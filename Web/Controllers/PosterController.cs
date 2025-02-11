using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class PosterController : Controller
    {
        // GET: Poster
        public ActionResult Index()
        {
            //var agent = Request.UserAgent.ToLower();
            ////mozilla/5.0 (linux; android 11; sm-a725f) applewebkit/537.36 (khtml, like gecko) chrome/91.0.4472.120 mobile safari/537.36
            //// Check if IOS
            //if (agent.IndexOf("iphone") > -1)
            //{
            //    return RedirectPermanent("https://apps.apple.com/us/app/travel-oriental-mindoro-ph/id1570131703");
            //}
            //else if (agent.IndexOf("android") > -1)
            //{
            //    return RedirectPermanent("https://play.google.com/store/apps/details?id=app.travelorientalmindoro.ph");
            //}
            //else
            //{
            //    return RedirectPermanent("https://travelorientalmindoro.ph/");
            //}

            return View();

        }
        public ActionResult Agent()
        {
            var agent = Request.UserAgent.ToLower();
            ViewBag.Agent = agent;

            // Check if IOS
            return View();
        }
    }
}