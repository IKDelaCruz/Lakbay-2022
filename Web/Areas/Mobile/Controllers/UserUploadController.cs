using Infrastructure.Files;
using Microsoft.Web.Mvc;
using Project.Models.Models;
using Project.Repository.Repo;
using Project.Repository.Repo.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;

namespace Web.Areas.Mobile.Controllers
{
    public class UserUploadController : Controller
    {
        public ActionResult Index(string userGuid, bool isFromIos = false, bool showAddButton = true)
        {
            var user = UserRepo.Get(userGuid);
            if (user == null) return null;
            var id = user.Id;

            TempData["UserId"] = user.GUID;
            TempData["IsFromIOS"] = isFromIos;
            TempData["ShowAddButton"] = showAddButton;

            var files = UserUploadFileRepo.GetList(id);
            var list = new List<UserUploadFileViewModel>();

            if (files.Any())
            {
                list = files.Select(a => new UserUploadFileViewModel()
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Path = a.Path,
                    Type = a.Type,
                    Filename = a.Filename,
                    FileExtension = a.FileExtension,
                    FileSize = a.FileSize,
                    UploadBy = a.UploadBy,
                    DateUploaded = a.DateUploaded,
                    LastDateModified = a.LastDateModified,
                    Remarks = a.Remarks,
                    IsDeleted = a.IsDeleted,
                    Guid = a.Guid,
                    imgUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath.TrimEnd('/')}/Content/Data/MobileData/{a.UserId}/uploadedfiles/{a.Filename}"
                }).ToList();
            }

            return View(list);
        }


        public ActionResult Add(string userGuid, bool isFromIos)
        {
            TempData["IsFromIOS"] = isFromIos;
            var user = UserRepo.Get(userGuid);
            var userId = user.Id;
            var model = new UserUploadFileViewModel()
            {
                UserId = userId,
                UploadBy = userId,
                DateUploaded = DateTime.Now,

            };

            return View(model);
        }

        [HttpPost]
        public JsonResult JsonAdd(UserUploadFileViewModel model)
        {
            if (model.File != null)
            {
                var upload = FileManager.Upload(model.File, $"{model.UserId}/uploadedfiles");

                if (upload.Success)
                {
           
                    model.UploadBy = model.UserId;
                    model.UserId = model.UserId;

                    model.Filename = upload.ReturnParam.ToString();
                    model.Path = $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath.TrimEnd('/')}/Content/Data/MobileData/{model.UserId}/uploadedfiles/{model.Filename}";
                    model.FileExtension = Path.GetExtension(model.File.FileName);

                    var result = UserUploadFileRepo.Add(model.ToDto());
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ReturnValue() { Message = "Unable to upload file." }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new ReturnValue() { Message = "Please attach file." }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(string guid, bool isFromIos = false)
        {
            TempData["IsFromIOS"] = isFromIos;
            var record = UserUploadFileRepo.Get(guid);
            var model = new UserUploadFileViewModel();

            if (record != null)
            {
                model = model.ToModel(record);
            }

            return View(model);
        }
        [HttpPost]
        public JsonResult JsonUpdate(UserUploadFileViewModel model)
        {
            model.LastDateModified = DateTime.Now;
            if (model.File != null)
            {
                var upload = FileManager.Upload(model.File, $"{model.UserId}/uploadedfiles");

                if (upload.Success)
                {
                    model.Filename = upload.ReturnParam.ToString();
                    model.Path = $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath.TrimEnd('/')}/Content/Data/MobileData/{model.UserId}/uploadedfiles/{model.Filename}";
                    model.FileExtension = Path.GetExtension(model.File.FileName);

                    var result = UserUploadFileRepo.Update(model.ToDto());
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ReturnValue() { Message = "Unable to upload file." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var result = UserUploadFileRepo.Update(model.ToDto());
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult JsonDelete(string guid)
        {
            var img = UserUploadFileRepo.Get(guid);
            if (img != null)
            {
                var deleted = FileManager.Delete($"{img.UserId}/uploadedfiles", img.Filename).Success;
                if (deleted)
                {
                    var result = UserUploadFileRepo.Delete(guid);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            ReturnValue res = new ReturnValue() { Message = "Unable to delete file" };
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [AjaxOnly]
        [HttpPost]
        public ActionResult Delete(string guid)
        {
            var img = UserUploadFileRepo.Get(guid);
            if (img != null)
            {
                var deleted = FileManager.Delete($"{img.UserId}/uploadedfiles", img.Filename).Success;
                if (deleted)
                {
                    deleted = UserUploadFileRepo.Delete(img.Guid).Success;
                }
                return new JsonResult() { Data = deleted, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
            return new JsonResult() { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult DownloadFile(string path)
        {
            WebClient webClient = new WebClient();
            byte[] fileBytes = webClient.DownloadData(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(path));
        }

    }
}