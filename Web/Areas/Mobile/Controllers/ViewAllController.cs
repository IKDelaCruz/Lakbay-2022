using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using static Project.Models.Enums;

namespace Web.Areas.Mobile.Controllers
{
    public class ViewAllController : Controller
    {
        public ActionResult attractions(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Attraction, 0, 10);
            mun = PlaceRepo.FilterClosest(mun, longitude, latitude);
            var favorites = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favorites, longitude, latitude)).ToList(),
                Type = CategoryTypeEnum.Attraction,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            return View(vm);
        }

        public ActionResult shoppings(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Shop, 0, 10);
            mun = PlaceRepo.FilterClosest(mun, longitude, latitude);
            var favorites = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favorites, longitude, latitude)).ToList(),
                Type = CategoryTypeEnum.Shop,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            return View("attractions", vm);
        }

        public ActionResult restaurants(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Restaurant, 0, 10);
            mun = PlaceRepo.FilterClosest(mun, longitude, latitude);
            var favorites = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favorites, longitude, latitude)).ToList(),
                Type = CategoryTypeEnum.Restaurant,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            return View("attractions", vm);
        }
        public ActionResult diveshops(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.DiveShops, 0, 10);
            mun = PlaceRepo.FilterClosest(mun, longitude, latitude);
            var favorites = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favorites, longitude, latitude)).ToList(),
                Type = CategoryTypeEnum.DiveShops,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            return View("attractions", vm);
        }
        public ActionResult accommodations(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Accommodation, 0, 10);
            mun = PlaceRepo.FilterClosest(mun, longitude, latitude);
            var favorites = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favorites, longitude, latitude)).ToList(),
                Type = CategoryTypeEnum.Accommodation,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            return View("attractions", vm);
        }

        public ActionResult recreations(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Recreation, 0, 10);
            mun = PlaceRepo.FilterClosest(mun, longitude, latitude);
            var favorites = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favorites, longitude, latitude)).ToList(),
                Type = CategoryTypeEnum.Recreation,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            return View("attractions", vm);
        }

        public ActionResult events(int id, int year = 0, string longitude = "", string latitude = "")
        {
            year = year > 0 ? year : DateTime.Now.Year;

            var mun = PlaceRepo.GetListEvents().OrderByDescending(h=>h.DisplayPriority).ThenBy(h=>h.PublishedDate);
            var favorites = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = PlaceRepo.FilterClosest(mun, longitude, latitude).Select(s => new PlaceViewModel(s, favorites, longitude, latitude)).ToList(),
                Type = CategoryTypeEnum.NewsEvents,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            return View("events", vm);
        }
    }
}