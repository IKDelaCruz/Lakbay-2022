using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Database.DbContext;
using Project.Models.Models;
using static Project.Models.Enums;
using System.Data.Entity;

namespace Project.Repository.Repo
{
    public class PlaceImagesRepo
    {
        public static List<PlaceImagesDto> GetList(int placeId = 0, PlaceImagesType type = PlaceImagesType.All)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.PlaceImages.Where(a => a.PlaceId == placeId);
                if (type != PlaceImagesType.All)
                    list = list.Where(h => h.Type == (int)type);

                return list.Select(a => new PlaceImagesDto()
                {
                    Id = a.Id,
                    Filename = a.Filename,
                    PlaceId = a.PlaceId,
                    Type = (PlaceImagesType)a.Type,
                    ImageText = a.ImageText ?? "",
                    YoutubeUrl = a.YoutubeUrl
                    
                }).ToList();
            }
        }
        public static List<PlaceImagesDto> GetList(PlaceImagesType type = PlaceImagesType.All)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.PlaceImages.Include(s => s.Place).Where(s => !s.Place.IsDeleted);
                if (type != PlaceImagesType.All)
                    list = list.Where(h => h.Type == (int)type);

                return list.Select(a => new PlaceImagesDto()
                {
                    Id = a.Id,
                    Filename = a.Filename,
                    PlaceId = a.PlaceId,
                    Type = (PlaceImagesType)a.Type,
                    ImageText = a.ImageText ?? "",
                    YoutubeUrl = a.YoutubeUrl

                }).ToList();
            }
        }
    }
}
