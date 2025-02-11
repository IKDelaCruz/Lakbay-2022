using Infrastructure;
using Microsoft.AspNet.Identity;
using Project.Models.Models;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using static Project.Models.Enums;

namespace Web.Controllers
{
    public class PlacesController : BaseController
    {
        public ActionResult Accomodations(int id = 0)
        {
            return View(GetPlaces(CategoryTypeEnum.Accommodation, id));
        }
        public ActionResult Accommodations(int id = 0)
        {
            return View(GetPlaces(CategoryTypeEnum.Accommodation, id));
        }
        public ActionResult Search(string searchKeyword = "", int cityId = 0)
        {
            //var mun = PlaceRepo.GetListByType(0, 0, 10, param);

            return View(SearchPlaces(searchKeyword, cityId));
        }
        public ActionResult AccomodationsWidget(int id = 0)
        {
            IEnumerable<PlaceDto> mun;

            if (id > 0)
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Accommodation, id, 4);
            }
            else
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Accommodation, 0, 4);
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentId = id
            };
            return View(vm);
        }
        public ActionResult DivingWidget(int id = 0)
        {
            IEnumerable<PlaceDto> mun;

            if (id > 0)
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.DiveShops, id, 4);
            }
            else
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.DiveShops, 0, 4);
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentId = id
            };
            return View(vm);
        }
        public ActionResult Attractions(int id = 0)
        {
            return View(GetPlaces(CategoryTypeEnum.Attraction, id));
        }

        public ActionResult AttractionsWidget(int id = 0)
        {
            IEnumerable<PlaceDto> mun;

            if (id > 0)
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Attraction, id, 4);
            }
            else
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Attraction, 0, 4);
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentId = id
            };
            return View(vm);
        }

        public ActionResult Destination(int id)
        {
            var place = PlaceRepo.Get(id);
            place.ChildId = CityRepo.GetByPlaceId(place.Id).Id;
            if (place.HomeThumbnail == "")
            {
                place.HomeThumbnail = "default.jpg";
            }

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

        public ActionResult Destinations()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Municipality);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()
            };
            return View(vm);
        }

        public ActionResult DestinationsWidget()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Municipality);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList().OrderBy(h => h.Title).ToList()
            };
            return View(vm);
        }

        public ActionResult Details(int id)
        {
            var place = PlaceRepo.Get(id);
            var userId = 0;
            if (User.Identity.IsAuthenticated)
            {
                userId = UserId;//UserRepo.GetByEmail(User.Identity.Name).Id;
            }
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
                UserId = userId,
                ReviewViewModel = new ReviewViewModel(id, UserId, 999)
            };
            return View(vm);
        }

        public ActionResult Details_(string guid)
        {
            var place = PlaceRepo.Get(guid);
            return RedirectToAction("Details", new { id = place.Id });
        }

        public ActionResult Directory()
        {
            return View(GetPlaces(CategoryTypeEnum.DirectoryEntry, 0));
        }

        public ActionResult DiveShops(int id = 0)
        {

            return View(GetPlaces(CategoryTypeEnum.DiveShops, id));
        }

        // GET: Places
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MunicipalitiesWidget()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Municipality, 0, 4);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()
            };
            return View(vm);
        }

        public ActionResult Recreations(int id = 0)
        {
            return View(GetPlaces(CategoryTypeEnum.Recreation, id));
        }

        public ActionResult RecreationsWidget(int id = 0)
        {
            IEnumerable<PlaceDto> mun;
            if (id > 0)
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Recreation, id, 4);
            }
            else
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Recreation, 0, 4);
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentId = id
            };
            return View(vm);
        }

        public ActionResult Restaurants(int id = 0)
        {
            return View(GetPlaces(CategoryTypeEnum.Restaurant, id));
        }

        public ActionResult RestaurantsWidget(int id = 0)
        {
            IEnumerable<PlaceDto> mun;
            if (id > 0)
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Restaurant, id, 4);
            }
            else
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Restaurant, 0, 4);
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentId = id
            };
            return View(vm);
        }

        public ActionResult ReviewsWidget(int id = 0, int limit = 100)
        {
            return RedirectToAction("ReviewsWidget", "Reviews", new { id = id, limit = limit });
        }

        [HttpPost]
        public JsonResult Review(int id, int userId, int rating, string review)
        {
            string message = "";
            var user = UserRepo.Get(userId);
            if (user != null)
            {
                var dto = new ReviewDto()
                {
                    Username = user.Username,
                    ReviewStars = rating,
                    ReviewText = review,
                    PlaceId = id,
                    UserId = userId
                };

                message = ReviewRepo.Save(dto).Message;
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public FileResult SampleCreatePlaceQRCode(int id)
        {
            var path = Server.MapPath("/Content/img/Lakbay-small.png");
            var places = PlaceRepo.Get(id);
            Bitmap qr = Infrastructure.QR_Generator.QRGenerator.CreatePlaceQR(places, path);

            using (var stream = new System.IO.MemoryStream())
            {
                qr.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var bytes = stream.ToArray();

                return File(bytes, "image/jpeg");
            }
        }

        public ActionResult Shops(int id = 0)
        {
            return View(GetPlaces(CategoryTypeEnum.Shop, id));
        }
        private PlaceHomeViewModel GetPlaces(CategoryTypeEnum category, int parentId)
        {
            var mun = PlaceRepo.GetListByType((int)category, parentId, 0);
            var parentName = "";
            if (parentId > 0)
            {
                parentName = CityRepo.Get(parentId)?.Name ?? "";
            }

            int userId = UserId;
            var favList = FavoritesRepo.GetList(userId);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentName = parentName,
                Type = category,
                UserId = userId,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favList)).ToList()
            };
            return vm;
        }
        private PlaceHomeViewModel SearchPlaces(string param, int cityId = 0)
        {
            var mun = PlaceRepo.GetListByType(0, cityId, 20, param);
            var parentName = "";

            var favList = FavoritesRepo.GetList(UserId);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentName = parentName,
                Type = CategoryTypeEnum.SearchResult,
                SearchKeyword = param,
                UserId = UserId,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favList)).ToList()
            };
            return vm;
        }
        public ActionResult ShopsWidget(int id = 0)
        {
            IEnumerable<PlaceDto> mun;
            if (id > 0)
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Shop, id, 4);
            }
            else
            {
                mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.Shop, 0, 4);
            }

            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList(),
                ParentId = id
            };
            return View(vm);
        }

        public ActionResult TourGuides(int id = 0)
        {
            return View(GetPlaces(CategoryTypeEnum.TourGuides, id));
        }
        public ActionResult DRRMOffices()
        {
            return View(GetPlaces(CategoryTypeEnum.DRRMOOffice, 0));
        }
    }
}