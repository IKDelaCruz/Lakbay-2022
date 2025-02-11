using Project.Models;
using Project.Models.Models;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.MobileAdmin.Controllers
{
    public class AssignedPlaceController : Controller
    {
        public ActionResult Index(string guid)
        {
            var place = PlaceRepo.Get(guid);
            if (place == null)
                return RedirectToAction("ListPlaces");
            return View(place);
        }

        public ActionResult Index2(PlaceDto model)
        {
            return View(model);
        }

        public ActionResult ListPlaces()
        {
            var places = PlaceRepo.GetList();

            return View(places);
        }

        public ActionResult ListPlacesSearch(string param)
        {
            var types = new List<int>()
            {
                (int)Enums.CategoryTypeEnum.Accommodation,
                (int)Enums.CategoryTypeEnum.Attraction,
                (int)Enums.CategoryTypeEnum.Municipality,
                (int)Enums.CategoryTypeEnum.Recreation,
                (int)Enums.CategoryTypeEnum.Restaurant,
                (int)Enums.CategoryTypeEnum.Shop,
                (int)Enums.CategoryTypeEnum.DiveShops,
                (int)Enums.CategoryTypeEnum.Other,
                (int)Enums.CategoryTypeEnum.DRRMOOffice
            };

            var places = PlaceRepo.GetList(string.Join("|", types.Select(a => a.ToString())), param);
            return PartialView("_ListPlaces", places);
        }
    }
}