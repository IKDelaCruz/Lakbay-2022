using Microsoft.Web.Mvc;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.Mobile.ViewModels;
//using Web.Areas.Mobile.ViewModels;
using Web.ViewModel;
using Web.ViewModel.Profile;
using Web.ViewModel.Review;
using static Project.Models.Enums;

namespace Web.Areas.Mobile.Controllers
{
    public class ApiController : Controller
    {
        [AjaxOnly]
        [HttpPost]
        public ActionResult ToggleFavorites(int userid, int id, bool liked)
        {
            var success = FavoritesRepo.SetFavorites(userid, id, liked);
            return new JsonResult() { Data = success, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private List<int> _GetAvaliableMunicipalPlacesFromSelectedCategories(List<int> selectedCategories)
        {
            var list = PlaceRepo.GetAvaliableMunicipalPlacesFromSelectedCategories(selectedCategories);
            return list.ToList();
        }
       
        public ActionResult GetMunicipalList()
        {
            string types = System.Web.HttpContext.Current.Request.QueryString["cats"];
            if (string.IsNullOrEmpty(types))
            {
                types = ((int)CategoryTypeEnum.Attraction).ToString() + "|";
                types = types + ((int)CategoryTypeEnum.DiveShops).ToString() + "|";
                types = types + ((int)CategoryTypeEnum.Restaurant).ToString() + "|";
                types = types + ((int)CategoryTypeEnum.Recreation).ToString() + "|";
                types = types + ((int)CategoryTypeEnum.Accommodation).ToString() + "|";
                types = types + ((int)CategoryTypeEnum.NewsEvents).ToString() + "|";
                types = types + ((int)CategoryTypeEnum.Shop).ToString();
            }
            var selectedCategories = types.Split('|').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a)).ToList();

            var municipals = _GetAvaliableMunicipalPlacesFromSelectedCategories(selectedCategories);
            municipals.Add(0);
            var jResult = Json(new
            {
                data = municipals
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }


        public ActionResult FavoritesDatatable()
        {
            int UserId = int.Parse(System.Web.HttpContext.Current.Request.QueryString["userid"]);

            string longitude = System.Web.HttpContext.Current.Request.QueryString["longitude"];
            string latitude = System.Web.HttpContext.Current.Request.QueryString["latitude"];
            var favList = FavoritesRepo.GetList(UserId);

            var list = PlaceRepo.GetList(favList.Select(s => s.Place.Id).Distinct());
            list = PlaceRepo.FilterClosest(list, longitude, latitude);

            var PlacesViewModel = list.Select(s => new PlaceViewModel(s, favList, longitude, latitude)).ToList();
            var jResult = Json(new
            {
                data = PlacesViewModel.Select(s => new FavoritesViewModel(
                    this.Url,
                    System.Configuration.ConfigurationManager.AppSettings["CDNHOST"],
                    UserId,
                    s)
                ).ToList()
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }

        public ActionResult ReviewsDatatable()
        {
            int userId = int.Parse(System.Web.HttpContext.Current.Request.QueryString["userid"]);
            int id = int.Parse(System.Web.HttpContext.Current.Request.QueryString["id"]);

            var vm = new ViewModel.ReviewViewModel(id, userId, 9999);

            var jResult = Json(new
            {
                data = vm.PlaceReviews.Select(s => new ReviewDatatableItemViewModel(
                    this.Url,
                    System.Configuration.ConfigurationManager.AppSettings["CDNHOST"],
                    userId,
                    s)
                ).ToList()
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }


        [ValidateInput(false)]
        public ActionResult SearchByDestination(int id, string param, bool isFavourite, string longitude = "", string latitude = "", bool nearestMe = false)
        {
            var mun = PlaceRepo.GetListByType(0, 0, 10, param);
            if (nearestMe)
            {
                mun = PlaceRepo.FilterClosest(mun, longitude, latitude);
            }
            if (isFavourite)
            {
                var favs = FavoritesRepo.GetList(id).Select(s => s.Place.Id);
                mun = mun.Where(s => favs.Contains(s.Id)).ToList();
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id
            };
            var userId = id;

            var url = this.Url;
            var jResult = Json(new
            {
                data = mun.ToList().Select(s => new SearchResultItemViewModel(
                    url,
                    System.Configuration.ConfigurationManager.AppSettings["CDNHOST"],
                    userId,
                    s,
                    nearestMe
                    )
                ).ToList()
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }
    }
}