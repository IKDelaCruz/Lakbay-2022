using Project.Database.DbContext;
using Project.Models.API.User;
using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Project.Repository.Repo.Users
{
    public class UserPlaceVisitHistoryRepo
    {
        public static UserPlaceVisitHistoryDto ToDto(UserPlaceVisitHistory ent)
        {
            if (ent == null) return null;
            return new UserPlaceVisitHistoryDto
            {
                Id = ent.Id,
                UserId = ent.UserId,
                PlaceId = ent.PlaceId,
                DateVisit = ent.DateVisit,
                Remarks = ent.Remarks
            };
        }

        public static UserPlaceVisitHistory ToEntity(UserPlaceVisitHistoryDto dto, UserPlaceVisitHistory ent = null)
        {
            ent = ent ?? new UserPlaceVisitHistory();
            ent.UserId = dto.UserId;
            ent.PlaceId = dto.PlaceId;
            ent.DateVisit = dto.DateVisit;
            ent.Remarks = dto.Remarks;
            return ent;
        }

        public static bool Add(AddUserPlaceVisitModel a)
        {
            using (var context = new LakbayDBEntities())
            {
                var user = context.Users.FirstOrDefault(s => s.GUID == a.UserGuid);
                var place = context.Places.FirstOrDefault(s => s.GUID == a.PlaceGuid);

                if (user == null || place == null)
                    return false;


                var ent = new UserPlaceVisitHistory
                {
                    UserId = user.Id,
                    PlaceId = place.Id,
                    DateVisit = DateTime.Now,
                    Remarks = ""
                };
                context.UserPlaceVisitHistories.Add(ent);
                return context.SaveChanges() > 0;
            }
        }

        public static bool Add(UserPlaceVisitHistoryDto a)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = new UserPlaceVisitHistory
                {
                    UserId = a.UserId,
                    PlaceId = a.PlaceId,
                    DateVisit = DateTime.Now,
                    Remarks = a.Remarks
                };
                context.UserPlaceVisitHistories.Add(ent);
                return context.SaveChanges() > 0;
            }
        }

        public static IEnumerable<UserPlaceVisitModel> GetList(int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                return context.UserPlaceVisitHistories.Include(s => s.Place).Where(s => s.UserId == userId).ToList().OrderByDescending(s => s.Id).Select(s => new UserPlaceVisitModel
                {
                    dto = ToDto(s),
                    Place = PlaceRepo.ToDto(s.Place)
                }).ToList();
            }
        }
    }
}
