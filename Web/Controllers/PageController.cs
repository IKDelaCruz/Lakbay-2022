using Project.Models.Models;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using static Project.Models.Enums;

namespace Web.Controllers
{
    public class PageController : Controller
    {
        public ActionResult Custom(string name)
        {
            var place = PlaceRepo.GetByHomeSlug(name);
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult GovernorsMessage()
        {
            var place = PlaceRepo.GetByHomeSlug("governors-message");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult TravelAdvisory()
        {
            var place = PlaceRepo.GetByHomeSlug("travel-advisory");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        
        public ActionResult Lakbay()
        {
            var place = PlaceRepo.GetByHomeSlug("lakbay-oriental-mindoro");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult HowToGetToOrientalMindoro()
        {
            var place = PlaceRepo.GetByHomeSlug("how-to-get-to-oriental-mindoro");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult GovernorsMessageWidget()
        {
            int id = 32;
            var place = PlaceRepo.Get(id);

            var images = PlaceImagesRepo.GetList(id);
            foreach (PlaceImagesDto p in images)
            {
                if (p.Type == PlaceImagesType.YoutubeURL)
                {
                    Utilities.YoutubeHelper.SetupYoutube(p);
                }
            }
            var vm = new PlaceDetailViewModel
            {
                Place = place,
                Images = images

            };
            return View(vm);
        }
        public ActionResult History(string id)
        {
            var place = PlaceRepo.GetByHomeSlug(id);
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