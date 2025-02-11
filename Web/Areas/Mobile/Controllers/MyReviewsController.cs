using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel.Profile;
using Web.ViewModel.Review;

namespace Web.Areas.Mobile.Controllers
{
    public class MyReviewsController : Controller
    {
        // GET: Mobile/MyReviews
        public ActionResult Index(string Guid)
        {
            ViewData["Guid"] = Guid;
            return View();
        }

        public ActionResult DatatableReviews()
        {
            string userGuid = System.Web.HttpContext.Current.Request.QueryString["id"];
            var user = UserRepo.Get(userGuid);
            var UserId = user?.Id ?? 0;
            var list = ReviewRepo.GetListWithPlaceDetail(UserId);

            var jResult = Json(new
            {
                data = list.Select(s => new ProfileReviewDatatableItemViewModel(
                    this.Url,
                    System.Configuration.ConfigurationManager.AppSettings["CDNHOST"],
                    UserId,
                    s.Item2,
                    s.Item1)
                ).ToList()
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;
            return jResult;
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var message = ReviewRepo.Delete(id);
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReviewDetail(int id)
        {
            var a = ReviewRepo.Get(id);
            return PartialView("_ReviewDetail", new ReviewHistoryViewModel() { Place = a.Item1, Review = a.Item2 });
        }

        [HttpPost]
        public JsonResult UpdateReview(ReviewHistoryViewModel model)
        {
            var message = ReviewRepo.Save(model.Review);
            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}