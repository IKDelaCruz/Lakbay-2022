using Project.Models.Models;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using Web.ViewModel.Menu;
using static Project.Models.Enums;

namespace Web.Controllers
{
    public class MenuController : BaseController
    {
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Header()
        {
            var linkId = (TempData.Peek("LinkId") != null ? TempData.Peek("LinkId").ToString() : "");
            var linkType = (TempData.Peek("LinkType") != null ? TempData.Peek("LinkType").ToString() : "");

            var list = PlaceRepo.GetListHistory();
            var model = new HeaderViewModel();
            model.ListCityHistory = list.ToList();

            switch (linkType.ToString().ToLower())
            {
                case "facebook":
                case "google":
                    TempData["UserIdentity"] = UserRepo.GetByLinkId(linkId.ToString());
                    break;
                case "email":
                    TempData["UserIdentity"] = UserRepo.GetByEmail(linkId.ToString());
                    break;
                default:
                    break;
            }



            return View(model);
        }
        public ActionResult Footer()
        {
            return View();
        }
        public ActionResult MobileFooter()
        {
            return View();
        }
        public ActionResult SignInPopup()
        {
            return View();
        }
        public ActionResult Policy()
        {
            return View();
        }

        public ActionResult ShowAnnouncement(bool mobileView = false)
        {
            //mobileView = true;
            TempData["MobileView"] = mobileView;

            var type = mobileView ? PlaceImagesType.AnnouncementMobile : PlaceImagesType.AnnouncementWeb;
            var images = PlaceImagesRepo.GetList(type);
            var vm = new PlaceDetailViewModel
            {
                // Place = announcement[0],
                Images = images.OrderByDescending(s => s.Id).ToList(),
            };
               
            return PartialView("_Announcement", vm);
      
            //IEnumerable <AnnouncementModel> announcements = AnnouncementRepo.GetList(true);
            //TempData["MobileView"] = mobileView;
            //return PartialView("_Announcement", announcements);
        }
    }
}