using Project.Database.DbContext;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Project.Repository.Repo
{
    public class FavoritesRepo
    {
        private static FavoritesDto ToDto(Favorite a)
        {
            if (a == null) return null;
            return new FavoritesDto()
            {
                UserId = a.UserId,
                Place = PlaceRepo.ToDto(a.Place),
                Date = a.Date,
                Id = a.Id,
            };
        }

        private static Favorite ToEntity(FavoritesDto a, Favorite ent = null)
        {
            if (a == null) return null;
            ent = ent ?? new Favorite();
            ent.UserId = a.UserId;
            ent.PlaceId = a.Place.Id;
            ent.Date = a.Date;
            return ent;
        }

        public static bool IsFavorite(int id, int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                return (context.Favorites.FirstOrDefault(s => s.UserId == userId && s.PlaceId == id) != null);
            }
        }

        public static bool ToggleFavorites(int userId, int placeId)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.Favorites.FirstOrDefault(s => s.PlaceId == placeId && s.UserId == userId);
                if(ent == null)
                {
                    context.Favorites.Add(new Favorite
                    {
                        UserId = userId,
                        PlaceId = placeId,
                        Date = DateTime.Now
                    });
                }
                else
                {
                    context.Favorites.Remove(ent);
                }
                return context.SaveChanges() > 0;
            }
                
        }
        public static bool SetFavorites(int userId, int placeId, bool liked)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.Favorites.FirstOrDefault(s => s.PlaceId == placeId && s.UserId == userId);
                if (ent == null && liked)
                {
                    context.Favorites.Add(new Favorite
                    {
                        UserId = userId,
                        PlaceId = placeId,
                        Date = DateTime.Now
                    });
                }
                else if(ent != null && !liked)
                {
                    context.Favorites.Remove(ent);
                }
                return context.SaveChanges() > 0;
            }

        }

        public static bool Add(FavoritesDto a)
        {
            using (var context = new LakbayDBEntities())
            {
                context.Favorites.Add(ToEntity(a));
                return context.SaveChanges() > 0;
            }
        }
        public static FavoritesDto Get(int placeId, int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.Favorites.Include(s => s.Place).FirstOrDefault(s => s.PlaceId == placeId && s.UserId == userId);
                if (ent == null) return null;

                return ToDto(ent);
            }
        }

        public static FavoritesDto Get(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.Favorites.Include(s => s.Place).FirstOrDefault(s => s.Id == id);
                if (ent == null) return null;

                return ToDto(ent);
            }
        }

        public static IEnumerable<FavoritesDto> GetList(int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                return context.Favorites.Include(s => s.Place).Where(s => s.UserId == userId).ToList().Select(s => ToDto(s)).ToList();
            }
        }

        public static bool Delete(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.Favorites.FirstOrDefault(s => s.Id == id);
                if (ent == null) return false;

                context.Favorites.Remove(ent);

                return context.SaveChanges() > 0;
            }
        }
    }
}
