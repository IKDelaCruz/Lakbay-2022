using Infrastructure.Files;
using Project.Models.Models;
using Project.Models.Models.UserModels;
using Project.Repository.Repo;
using Project.Repository.Repo.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.MobileAdmin.Controllers
{
    public class GuestDetailController : Controller
    {
        // GET: MobileAdmin/GuestDetail
        public ActionResult Index(int placeId, string guestGuid, string userGuid, bool isFirstRun = false)
        {
            userGuid = userGuid ?? "";
            guestGuid = guestGuid ?? "";

            TempData["UserGUID"] = userGuid;
            TempData["IsFirstRun"] = isFirstRun;
            TempData["PlaceID"] = placeId;
            MobileAuthUserModel user = (MobileAuthUserModel)(UserRepo.GetUserByQR(guestGuid).ReturnParam ?? new MobileAuthUserModel());

            if (user.Id > 0 && string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                var image = UserDetailRepo.GetProfileImage(user?.Id ?? 0);

                if (image == null)
                {
                    user.ProfilePicturePath = "~/Content/img/user_default_icon.png";
                }
                else
                {
                    string filePath = Path.Combine(UserDetailRepo.GetProfilePictureFolderPath(user?.Guid ?? ""), image.SystemName);
                    
                    if (!FileManager.IsFileExists(FileManager.GetVirtualPath(filePath)))
                    {
                        user.ProfilePicturePath = System.Web.Hosting.HostingEnvironment.MapPath("/Content/img/user_default_icon.png");
                    }
                    
                    user.ProfilePicturePath = FileManager.GetVirtualPath(filePath, fullPath: false);
                }
            }

            MobileAuthUserModel model = user != null ? user : null;

            //if (model == null)
            //{
            //    model = new MobileAuthUserModel()
            //    {
            //        FirstName = "Richmond Mendoza",
            //        Gender = "Male",
            //        Age = 29,
            //        Phone = "09457868238",
            //        Country = "Philippines",
            //        City = "Makati City",
            //    };
            //}

            return View(model);
        }

        public JsonResult SaveVisitHistory(UserPlaceVisitHistoryDto model)
        {
            model.DateVisit = DateTime.Now;
            var success = UserPlaceVisitHistoryRepo.Add(model);

            var result = new ReturnValue("Unable to save changes.", success);
            if (result.Success)
                result.Message = "Guest has been successfully checked in to this place.";

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}