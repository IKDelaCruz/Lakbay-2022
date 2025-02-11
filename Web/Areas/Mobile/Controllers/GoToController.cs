using Infrastructure;
using Microsoft.Web.Mvc;
using Project.Models.Models;
using Project.Repository.Repo;
using Project.Repository.Repo.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.Mobile.ViewModels;
using Web.ViewModel;
using static Project.Models.Enums;

namespace Web.Areas.Mobile.Controllers
{
    public class GoToController : Controller
    {
        public ActionResult SearchResult(string place = "", int userId = 0)
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Municipality, 0, 10);

            if (!string.IsNullOrEmpty(place))
            {
                mun = mun.Where(a => a.Title.ToLower().Contains(place.ToLower()));
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = userId
            };
            return View(vm);
        }

        public ActionResult Explore(int id)
        {
            ViewBag.UserId = id;
            return View();
        }

        public ActionResult Destination(int id)
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Municipality, 0, 16);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id
            };
            return View(vm);
        }

        public ActionResult DestinationDetails(int id, int userId)
        {
            var place = PlaceRepo.Get(id);

            var images = PlaceImagesRepo.GetList(id);
            foreach (PlaceImagesDto p in images)
            {
                if (p.Type == PlaceImagesType.YoutubeURL)
                {
                    Utilities.YoutubeHelper.SetupYoutube(p);
                    //p.YouTubeId = YoutubeHelper.GetIdFromUrl(p.YoutubeUrl);
                    //p.YoutubeThumbnail = YoutubeHelper.GetThumbnailFromId(p.YouTubeId);
                    p.ImageText = p.YoutubeThumbnail;

                    //var data = p.ImageText.Split(',');
                    //if (data.Length > 1)
                    //{
                    //    p.YouTubeId = data[0].Trim();
                    //    p.ImageText = data[1].Trim();
                    //}

                }
            }
            var vm = new PlaceDetailViewModel
            {
                Place = place,
                Images = images,
                UserId = userId
            };
            return View("PlaceDetails", vm);
        }
        public ActionResult Categories(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListByType(0, 0, 10);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                UserLongitude = longitude,
                UserLatitude = latitude
            };

            return View(vm);
        }

        public ActionResult CategoriesDetails(int userid, string types, int municipalid = 0, string longitude = "", string latitude = "")
        {
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

            var categories = types.Split('|').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a)).ToList();
            var mun = PlaceRepo.GetList(categories, municipalid);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = userid,
                UserLatitude = latitude,
                UserLongitude = longitude
            };
            var availableMunicipals = PlaceRepo.GetAvaliableMunicipalPlacesFromSelectedCategories(categories);
            var municipalList = PlaceRepo.GetListMunicipalities().ToList();
            TempData["AvaiableMunicipalities"] = municipalList.Where(s => availableMunicipals.Contains(s.Id)).ToList();
            TempData["Municipalities"] = municipalList;
            TempData["SelectedCategory"] = categories;
            TempData["SelectedMunicipality"] = municipalid;
            return View(vm);
        }

        public ActionResult CategoriesDetailsListView(int userid, string types, int municipalid = 0)
        {
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

            var categories = types.Split('|').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a)).ToList();
            var mun = PlaceRepo.GetList(categories, municipalid);
            mun = mun.OrderByDescending(h => h.DisplayPriority).ThenBy(h => h.PublishedDate).ThenBy(h => h.Title);

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = userid
            };

            TempData["Municipalities"] = PlaceRepo.GetListMunicipalities().ToList();
            TempData["SelectedCategory"] = categories;
            return PartialView("_CategoriesDetailsListView", vm);
        }

        public ActionResult CategoriesDetailsDatatable()
        {
            int userid = int.Parse(System.Web.HttpContext.Current.Request.QueryString["userid"]);
            string types = System.Web.HttpContext.Current.Request.QueryString["cats"];

            string longitude = System.Web.HttpContext.Current.Request.QueryString["longitude"];
            string latitude = System.Web.HttpContext.Current.Request.QueryString["latitude"];

            int municipalid = 0;
            int.TryParse(System.Web.HttpContext.Current.Request.QueryString["cits"], out municipalid);
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

            var categories = types.Split('|').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a)).ToList();
            var mun = PlaceRepo.GetList(categories, municipalid);
            PlaceRepo.ComputeDistanceFromUser(mun, longitude, latitude);

            //todo move to javascrupt
            mun = mun.OrderBy(s => s.MeterDistanceFromUser).ThenByDescending(h => h.DisplayPriority).ThenBy(h => h.PublishedDate).ThenBy(h => h.Title);


            var favoritesList = FavoritesRepo.GetList(userid);
            var url = this.Url;
            var jResult = Json(new
            {
                data = mun.ToList().Select(s => new CategoriesDetailViewModel(
                    url,
                    s,
                    System.Configuration.ConfigurationManager.AppSettings["CDNHOST"],
                    userid,
                    favoritesList,
                    longitude,
                    latitude)
                ).ToList()
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }


        public ActionResult Favorites(int id, string longitude = "", string latitude = "")
        {
            var mun = PlaceRepo.GetListFavorites(id);
            var favList = FavoritesRepo.GetList(id);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                UserId = id,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favList)).ToList(),
                UserLongitude = longitude,
                UserLatitude = latitude
            };
            ViewBag.UserId = id;
            return View(vm);
        }

        public ActionResult Details(int id, int userId)
        {
            var place = PlaceRepo.Get(id);
            // //
            var images = PlaceImagesRepo.GetList(id);
            foreach (PlaceImagesDto p in images)
            {
                if (p.Type == PlaceImagesType.YoutubeURL)
                {
                    Utilities.YoutubeHelper.SetupYoutube(p);
                    p.ImageText = p.YoutubeThumbnail;
                    //var videoId = p.Filename.Replace("https://www.youtube.com/watch?v=", "");
                    //p.ImageText = "https://img.youtube.com/vi/" + videoId + "/hqdefault.jpg";
                }
            }
            var vm = new PlaceDetailViewModel
            {
                Place = place,
                Images = images,
                UserId = userId
            };
            return View(vm);
        }

        public ActionResult Detailsg(string id, int userId)
        {
            var place = PlaceRepo.Get(id);
            if (place != null)
            {
                return RedirectToAction("Details", new { id = place.Id, userId = userId });
            }
            else
            {
                return RedirectToAction("Details", new { id = 0, userId = userId });
            }

        }

        public ActionResult EventDetails(int id)
        {
            var place = PlaceRepo.Get(id);
            // //
            var images = PlaceImagesRepo.GetList(id);
            foreach (PlaceImagesDto p in images)
            {
                if (p.Type == PlaceImagesType.YoutubeURL)
                {
                    Utilities.YoutubeHelper.SetupYoutube(p);
                    p.ImageText = p.YoutubeThumbnail;
                    //var videoId = p.Filename.Replace("https://www.youtube.com/watch?v=", "");
                    //p.ImageText = "https://img.youtube.com/vi/" + videoId + "/hqdefault.jpg";
                }
            }
            var vm = new PlaceDetailViewModel
            {
                Place = place,
                Images = images

            };
            return View(vm);
        }

        public ActionResult AboutUs()
        {
            var place = PlaceRepo.GetByHomeSlug("governors-message");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,


            };
            return View(vm);
        }
        public ActionResult Lakbay()
        {
            var place = PlaceRepo.GetByHomeSlug("lakbay-oriental-mindoro");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,


            };
            return View(vm);
        }
        public ActionResult TurismoTVMobile()
        {
            var place = PlaceRepo.GetByHomeSlug("turismo-tv");
            if (place == null)
                return Redirect("/");

            var images = PlaceImagesRepo.GetList(place.Id);
            foreach (PlaceImagesDto p in images)
            {
                if (p.Type == PlaceImagesType.YoutubeURL)
                {
                    Utilities.YoutubeHelper.SetupYoutube(p);
                }
            }
            var vm = new PlaceDetailViewModel
            {
                Place = place,
                Images = images
            };
            return View(vm);
        }
        public ActionResult PrivacyPolicy()
        {
            var place = PlaceRepo.GetByHomeSlug("privacy-policy");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult TermsAndConditions()
        {
            var place = PlaceRepo.GetByHomeSlug("terms-and-condition");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult TravelAdvisory()
        {
            var place = PlaceRepo.GetByHomeSlug("travel-advisory");
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }
        public ActionResult Directories()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.DirectoryEntry, 0, 0);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("Directories", vm);
        }
        public ActionResult DRRMOffices()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.DRRMOOffice, 0, 0);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View("DRRMOffices", vm);
        }

        public ActionResult TourGuide()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.TourGuides, 0, 0);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                Type = CategoryTypeEnum.TourGuides

            };
            return View("Directories", vm);
        }

        [ValidateInput(false)]
        public ActionResult SearchByDestination(int id, string param, bool isFavourite, string longitude = "", string latitude = "")
        {
            //var mun = PlaceRepo.GetListByType(0, 0, 10, param);

            //if (isFavourite)
            //{
            //    var favs = FavoritesRepo.GetList(id).Select(s => s.Place.Id);
            //    mun = mun.Where(s => favs.Contains(s.Id)).ToList();
            //}

            //var vm = new PlaceHomeViewModel
            //{
            //    Places = mun.ToList(),
            //    UserId = id
            //};

            var vm = new SearchResultsViewModel
            {
                IsFavourite = isFavourite,
                UserId = id,
                param = param,
                longitude = longitude,
                latitude = latitude,
                showNearest = true
            };


            return View("SearchResults", vm);
        }


        public ActionResult UserPlaceHistory(int userId)
        {
            var list = UserPlaceVisitHistoryRepo.GetList(userId);
            return View(new UserPlaceVisitHistoryViewModel
            {
                List = list.ToList(),
                UserId = userId
            });
        }
        public ActionResult UserPlaceHistoryByGUID(string guid)
        {
            var get = UserRepo.GetUserByQR(guid).ReturnParam;
            MobileAuthUserModel user = get != null ? (MobileAuthUserModel)get : null;
            if (user != null)
            {
                var list = UserPlaceVisitHistoryRepo.GetList(user.Id);
                return View("UserPlaceHistory", new UserPlaceVisitHistoryViewModel
                {
                    List = list.ToList(),
                    UserId = user.Id
                });
            }
            else
            {
                return View("UserPlaceHistory", new UserPlaceVisitHistoryViewModel
                {
                    List = new List<Project.Models.Models.UserModels.UserPlaceVisitModel>(),
                    UserId = 0
                });
            }
        }

        public ActionResult History(string id)
        {
            var place = PlaceRepo.GetByHomeSlug(id);
            if (place == null)
                return Redirect("/");

            var vm = new PlaceDetailViewModel
            {
                Place = place,

            };
            return View(vm);
        }














        [AjaxOnly]
        [HttpPost]
        public JsonResult UpdateFavorites(int userId, int placeId)
        {
            var success = FavoritesRepo.ToggleFavorites(userId, placeId);
            return Json(success, JsonRequestBehavior.AllowGet);
        }


        // [AjaxOnly]
        [HttpPost]
        public JsonResult AddToFavourites(int userId, int placeId, bool isFavourite)
        {
            string message = "";
            if (userId == 0)
            {
                message = "Unable to add to favorites at this time. Invalid user details. Thank You.";
            }
            else if (!isFavourite)
            {
                var dto = new FavoritesDto()
                {
                    Place = new PlaceDto() { Id = placeId },
                    UserId = userId,
                    Date = DateTime.Now
                };

                message = FavoritesRepo.Add(dto) ? "Successfully added to favorites." : "Unable to add to favorites at this time. Please try again later. Thank you.";
            }
            else
            {
                var fav = FavoritesRepo.Get(placeId, userId);
                if (fav != null)
                    message = FavoritesRepo.Delete(fav.Id) ? "Successfully removed from your favourite list." : "Unable to remove from your favourite list at this time. Please try again later. Thank you.";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}