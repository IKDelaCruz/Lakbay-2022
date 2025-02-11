using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //IEnumerable<Project.Models.Models.AnnouncementModel> announcements = AnnouncementRepo.GetList(true);
            return View();
        }

       
        public ActionResult Privacy()
        {
            var place = PlaceRepo.GetByHomeSlug("privacy-policy");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult Terms()
        {
            var place = PlaceRepo.GetByHomeSlug("terms-and-condition");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
    }
}