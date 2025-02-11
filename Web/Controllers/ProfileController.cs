using Infrastructure.Files;
using Microsoft.Web.Mvc;
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
using Web.ViewModel;
using Web.ViewModel.Profile;
using Web.ViewModel.Review;
using Web.ViewModel.Upload;

namespace Web.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        [Authorize]
        // GET: Profile
        public ActionResult Index()
        {
            var path = Server.MapPath(Url.Content("~/Content/assets/CountriesList.json"));
            var userDetails = UserDetailRepo.GetByUserId(UserId);
            var vm = new ProfileViewModel(path, userDetails, GUID);
            ViewBag.Message = TempData["message"]?.ToString() ?? "";
            return View(vm);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Index(ProfileViewModel model)
        {
            var suc = UserDetailRepo.Update(model.Dto, model.UploadProfilePic);
            if (suc.Success)
            {
                TempData["message"] = suc.Message;
                return RedirectToAction("Index");
            }
            else
            {
                model.countriesListPath = Server.MapPath(Url.Content("~/Content/assets/CountriesList.json"));
                ViewBag.Message = suc.Message;
                return View("Index", model);
            }
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            var vm = new ChangePasswordViewModel();
            ViewBag.Message = TempData["message"]?.ToString() ?? "";
            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel vm)
        {
            var auth = AuthenticationRepo.Authenticate(UserId, vm.Password);
            if (!auth)
            {
                ViewBag.Message = "Incorrect password";
                vm.NewPassword = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }
            if (vm.NewPassword != vm.ConfirmPassword)
            {
                ViewBag.Message = "New password does not match confirm password";
                vm.NewPassword = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.NewPassword))
            {
                ViewBag.Message = "Please enter a new password";
                vm.NewPassword = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }

            var change = AuthenticationRepo.ChangePassword(new Project.Models.API.Authentication.ChangePasswordModel
            {
                UserId = UserId,
                NewPassword = vm.NewPassword
            });
            if (change.Success)
            {
                TempData["message"] = change.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = change.Message;
                return View(vm);
            }

        }
        [Authorize]
        public ActionResult Favorites()
        {
            var favList = FavoritesRepo.GetList(UserId);
            var list = PlaceRepo.GetList(favList.Select(s => s.Place.Id).Distinct());
            var vm = new PlaceHomeViewModel
            {
                Places = list.ToList(),
                ParentName = "",
                UserId = UserId,
                PlacesViewModel = list.Select(s => new PlaceViewModel(s, favList)).ToList()
            };
            return View(vm);
        }
        [Authorize]
        public ActionResult Favorites2()
        {
            var favList = FavoritesRepo.GetList(UserId);
            var list = PlaceRepo.GetList(favList.Select(s => s.Place.Id).Distinct());
            var vm = new PlaceHomeViewModel
            {
                Places = list.ToList(),
                ParentName = "",
                UserId = UserId,
                PlacesViewModel = list.Select(s => new PlaceViewModel(s, favList)).ToList()
            };
            return View(vm);
        }

        [Authorize]
        public ActionResult History()
        {
            return View(new UserPlaceVisitHistoryViewModel
            {
                UserId = UserId
            });
        }
        [Authorize]
        public ActionResult History2()
        {
            var list = UserPlaceVisitHistoryRepo.GetList(UserId);
            return View(new UserPlaceVisitHistoryViewModel
            {
                List = list.ToList(),
                UserId = UserId
            });
        }

        [Authorize]
        [AjaxOnly]
        public ActionResult HistoryTable()
        {
            var list = UserPlaceVisitHistoryRepo.GetList(UserId);

            var jResult = Json(new
            {
                data = list.Select(s => new HistoryTableItemViewModel(
                    this.Url,
                    System.Configuration.ConfigurationManager.AppSettings["CDNHOST"],
                    UserId,
                    s)
              ).ToList()
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }


        [AjaxOnly]
        [HttpPost]
        public ActionResult GetCities(string Country)
        {
            var path = Server.MapPath(Url.Content("~/Content/assets/CountriesList.json"));
            return new JsonResult() { Data = ProfileViewModel.GetCities(path, Country), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private FileResult GetProfilePicture(int id, bool isThumbnail = false)
        {
            var user = UserRepo.Get(id);
            return GetProfilePicture(user, isThumbnail);
        }
        private FileResult GetProfilePicture(string guid, bool isThumbnail = false)
        {
            var user = UserRepo.Get(guid);
            return GetProfilePicture(user, isThumbnail);
        }
        private FileResult GetProfilePicture(UserDto user, bool isThumbnail = false)
        {
            var defaultIconPath = System.Web.Hosting.HostingEnvironment.MapPath("/Content/img/user_default_icon.png");
            var path = UserRepo.GetProfilePicturePath(defaultIconPath, user, isThumbnail);
            return File(path, MimeMapping.GetMimeMapping(path));
        }


        [HttpGet]
        [Authorize]
        public FileResult ProfilePicture(string guid, bool isThumbnail = false)
        {
            return GetProfilePicture(guid, isThumbnail);
        }

        [HttpGet]
        [AllowAnonymous]
        public FileResult ProfilePictureById(int id, bool isThumbnail = false)
        {
            return GetProfilePicture(id, isThumbnail);
        }


        [Authorize]
        public ActionResult Reviews()
        {
            return View("ReviewHistory");
        }

        [Authorize]
        public ActionResult GetReviews()
        {
            var list = ReviewRepo.GetListWithPlaceDetail(UserId);
            return PartialView("_ReviewList", list.Select(a => new ReviewHistoryViewModel() { Place = a.Item1, Review = a.Item2 }));
        }

        [Authorize]
        public ActionResult DatatableReviews()
        {
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

        [Authorize]
        public ActionResult ReviewDetail(int id)
        {
            var a = ReviewRepo.Get(id);
            return PartialView("_ReviewDetail", new ReviewHistoryViewModel() { Place = a.Item1, Review = a.Item2 });
        }

        [Authorize]
        public JsonResult UpdateReview(ReviewHistoryViewModel model)
        {
            var message = ReviewRepo.Save(model.Review);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult Delete(int id)
        {
            var message = ReviewRepo.Delete(id);
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Upload()
        {
            return View("UserDocumentUpload");
        }

        [Authorize]
        public ActionResult GetUploadedList()
        {
            var list = UserUploadFileRepo.GetList(UserId);
            var list2 = new List<UserUploadFileViewModel>();
            list2 = list.Select(a => new UserUploadFileViewModel()
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
            return PartialView("_UploadedDocumentList", list2);
        }

        [Authorize]
        public ActionResult UploadedDocument(int id)
        {
            var model = new UserUploadFileViewModel()
            {
                UserId = UserId,
                UploadBy = UserId,
                DateUploaded = DateTime.Now,
            };

            if (id > 0)
            {
                var record = UserUploadFileRepo.Get(id);
                model = model.ToModel(record);
            }

            return PartialView("_UploadedDocumentDetail", model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadedDocument(UserUploadFileViewModel model)
        {
            if (model.Id > 0)
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
                        TempData["Message"] = result.Message;
                        TempData["MessageType"] = result.Success ? "success" : "error";
                    }
                    else
                    {
                        TempData["Message"] = "Unable to upload file.";
                        TempData["MessageType"] = "error";
                    }
                }
                else
                {
                    var result = UserUploadFileRepo.Update(model.ToDto());
                    TempData["Message"] = result.Message;
                    TempData["MessageType"] = result.Success ? "success" : "error";
                }
            }
            else
            {
                if (model.File != null)
                {
                    var upload = FileManager.Upload(model.File, $"{model.UserId}/uploadedfiles");

                    if (upload.Success)
                    {
                        model.Filename = upload.ReturnParam.ToString();
                        model.Path = $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath.TrimEnd('/')}/Content/Data/MobileData/{model.UserId}/uploadedfiles/{model.Filename}";
                        model.FileExtension = Path.GetExtension(model.File.FileName);

                        var result = UserUploadFileRepo.Add(model.ToDto());
                        TempData["Message"] = result.Message;
                        TempData["MessageType"] = result.Success ? "success" : "error";
                    }
                    else
                    {
                        TempData["Message"] = "Unable to upload file.";
                        TempData["MessageType"] = "error";
                    }
                }
                else
                {
                    TempData["Message"] = "Please select a valid file to upload.";
                    TempData["MessageType"] = "error";
                }
            }

            return RedirectToAction("Upload");
        }
        
        [Authorize]
        public ActionResult DeleteUploadedDocument(int id)
        {
            var result = UserUploadFileRepo.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction("Upload");
        }



        [Authorize]
        public ActionResult DatatableUserDocuments()
        {
            var list = UserUploadFileRepo.GetList(UserId);
        
            var list2 = list.OrderByDescending(s => s.DateUploaded).Select(a => new DatatableUserUploadFileViewModel()
            {
                t = a,
                imgUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath.TrimEnd('/')}/Content/Data/MobileData/{a.UserId}/uploadedfiles/{a.Filename}"
            }).ToList();


            var jResult = Json(new
            {
                data = list2
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }





    }

}