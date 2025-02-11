using Project.Models.Models;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using Web.ViewModel.Review;
using static Project.Models.Enums;

namespace Web.Areas.Mobile.Controllers
{
    public class WidgetsController : Controller
    {
        public ActionResult Municipalities(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = false;
            ViewBag.MainTitle = "Incredible Destinations";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/municipalities";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Municipality);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("WidgetContainer", vm);
        }

        public ActionResult Attractions(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = true;
            ViewBag.MainTitle = "Popular Attractions";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/attractions";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Attraction);
            if (id > 0)
                mun = mun.Where(h => h.ParentId == id);

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()
            };
            return View("WidgetContainer", vm);
        }

        public ActionResult AttractionsCarousel(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = true;
            ViewBag.MainTitle = "Similar Places";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/attractions";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Attraction, id, 10);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = userId
            };
            return View("PlaceOwlCarousel", vm);
        }

        public ActionResult Shoppings(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = true;
            ViewBag.MainTitle = "Popular Shops";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/shops";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Shop);
            if (id > 0)
                mun = mun.Where(h => h.ParentId == id);

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("WidgetContainer", vm);
        }

        public ActionResult FoodAndDinings(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = true;
            ViewBag.MainTitle = "Popular Restaurants";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/foodanddinings";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Restaurant);
            if (id > 0)
                mun = mun.Where(h => h.ParentId == id);

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("WidgetContainer", vm);
        }

        public ActionResult Accommodations(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = true;
            ViewBag.MainTitle = "Popular Accommodations";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/accommodations";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Accommodation);
            if (id > 0)
                mun = mun.Where(h => h.ParentId == id);

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("WidgetContainer", vm);
        }

        public ActionResult Tours(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = true;
            ViewBag.MainTitle = "Popular Recreations";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/recreations";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Recreation);
            if (id > 0)
                mun = mun.Where(h => h.ParentId == id);

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("WidgetContainer", vm);
        }

        public ActionResult Dives(int id = 0, int userId = 0)
        {
            ViewBag.ShowDevider = true;
            ViewBag.MainTitle = "Events and festivals";
            ViewBag.Description = "";
            ViewBag.ViewAllUrl = "/mobile/goto/events";

            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.NewsEvents);
            if (id > 0)
                mun = mun.Where(h => h.ParentId == id);

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("Events", vm);
        }
        
        public ActionResult Events(int id = 0)
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.NewsEvents, 0, 10);
            var favList = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favList)).ToList()
            };
            return View("All_Events", vm);
        }

        public ActionResult Review(int id, int userId = 0, int limit = 5)
        {
            var vm = new ReviewViewModel(id, userId, limit);
            return View(vm);
        }
        public ActionResult ReviewPartialView(int id, int userId = 0, int limit = 5)
        {
            var vm = new ReviewViewModel(id, userId, limit);
            return PartialView("Review",vm);
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

        [HttpPost]
        public ActionResult DeleteReview(int id)
        {
            var message = ReviewRepo.Delete(id);
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Review(int id, int placeid, int userId, int rating, string review)
        {
            var retVal = new ReturnValue();
            var user = UserRepo.Get(userId);
            if (user != null)
            {
                var dto = new ReviewDto()
                {
                    Id = id,
                    Username = user.Username,
                    ReviewStars = rating,
                    ReviewText = review,
                    PlaceId = placeid,
                    UserId = userId
                };

                retVal = ReviewRepo.Save(dto);
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }
    }
}