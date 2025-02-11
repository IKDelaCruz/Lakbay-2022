using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;
using static Project.Models.Enums;
using Project.Models.Models;
using Project.Repository.Repo;


namespace Web.Controllers
{
    public class FestivalController : BaseController
    {
        // GET: Festival
        public ActionResult Index()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.NewsEvents);
            int userId = UserId;
            var favList = FavoritesRepo.GetList(userId);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList().OrderByDescending(h => h.DisplayPriority).ThenBy(h => h.PublishedDate).ToList(),
                Type = CategoryTypeEnum.NewsEvents,
                PlacesViewModel = mun.Select(s => new PlaceViewModel(s, favList)).ToList(),
                UserId = userId

            };
            return View(vm);
        }
        public ActionResult HomeWidget()
        {
            var mun = PlaceRepo.GetListByType((int)CategoryTypeEnum.NewsEvents,0 , 3);
            var vm = new PlaceHomeViewModel
            {
                Places = mun.ToList()

            };
            return View(vm);
        }
        public ActionResult Details(int id)
        {
            var place = PlaceRepo.Get(id);
            // //
            var images = PlaceImagesRepo.GetList(id);
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
    }
}

