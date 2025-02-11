using Microsoft.Web.Mvc;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using Web.ViewModel.Profile;

namespace Web.Controllers
{
    public class FavoritesApiController : BaseController
    {
        [AjaxOnly]
        [HttpPost]
        public ActionResult ToggleFavorites(int userid, int id, bool liked)
        {
            var success = FavoritesRepo.SetFavorites(userid, id, liked);
            return new JsonResult() { Data = success, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public ActionResult FavoritesDatatable()
        {
            int userid = int.Parse(System.Web.HttpContext.Current.Request.QueryString["userid"]);
            var _userId = UserId;

            var favList = FavoritesRepo.GetList(UserId);
         
            var list = PlaceRepo.GetList(favList.Select(s => s.Place.Id).Distinct());
            var PlacesViewModel = list.Select(s => new PlaceViewModel(s, favList)).ToList();
            var jResult = Json(new
            {
                data = PlacesViewModel.Select(s => new FavoritesTableItemViewModel(
                    this.Url,
                    System.Configuration.ConfigurationManager.AppSettings["CDNHOST"],
                    userid,
                    s)
                ).ToList()
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }
    }
}