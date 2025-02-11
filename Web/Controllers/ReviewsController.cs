using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using Web.ViewModel.Review;

namespace Web.Controllers
{
    public class ReviewsController : BaseController
    {
        public ActionResult ReviewsWidget(int id = 0, int limit = 100)
        {
            //throw new Exception("replaced");
            var vm = new ViewModel.ReviewViewModel(id, UserId, limit);
            return View(vm);
        }

        public ActionResult ReviewsDatatable()
        {
            int userId = UserId;// int.Parse(System.Web.HttpContext.Current.Request.QueryString["userid"]);
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
    }
}