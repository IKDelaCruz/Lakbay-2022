using Infrastructure;
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
    public class TurismoTVController : Controller
    {
        // GET: TurismoTV
        public ActionResult Index()
        {
            var place = PlaceRepo.GetByHomeSlug("turismo-tv");
            if (place == null)
                return Redirect("/");

            var images = PlaceImagesRepo.GetList(place.Id);
            foreach (PlaceImagesDto p in images)
            {
                if (p.Type == PlaceImagesType.YoutubeURL)
                {
                    Utilities.YoutubeHelper.SetupYoutube(p);

                    //var videoId = p.Filename.Replace("https://www.youtube.com/watch?v=", "");
                    //p.ImageText = "https://img.youtube.com/vi/" + videoId + "/hqdefault.jpg";
                }
            }
            var vm = new PlaceDetailViewModel
            {
                Place = place,
                Images = images
            };
            return View(vm);
        }
        public ActionResult Videos()
        {
            var place = PlaceRepo.GetByHomeSlug("turismo-tv");
            if (place == null)
                return Redirect("/");

            var images = PlaceImagesRepo.GetList(place.Id);
            foreach (PlaceImagesDto p in images)
            {
                if (p.Type == PlaceImagesType.YoutubeURL)
                {
                    Utilities.YoutubeHelper.SetupYoutube(p);
                    //var videoId = p.Filename.Replace("https://www.youtube.com/watch?v=", "");
                    //p.ImageText = "https://img.youtube.com/vi/" + videoId + "/hqdefault.jpg";
                }
            }
            var vm = new PlaceDetailViewModel
            {
                Place = place,
                Images = images
            };
            return View(vm);
        }
    }
}