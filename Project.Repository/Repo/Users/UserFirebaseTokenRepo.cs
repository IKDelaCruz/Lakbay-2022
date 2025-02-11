//using FirebaseCloudPushMessagingLibrary2;
using Project.Database.DbContext;
using Project.Models.Models;
using Project.Models.Models.NotificationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repository.Repo.Users
{
    public class UserFirebaseTokenRepo
    {
        internal static UserFirebaseTokenDto ToDto(FirebaseUserToken a)
        {
            if (a == null) return null;
            return new UserFirebaseTokenDto()
            {
                UserId = a.UserId,
                FirebaseToken = a.Token,
                Id = a.Id,
                DeviceId = a.DeviceId,
                LastUpdate = a.LastUpdate
            };
        }

        internal static FirebaseUserToken ToEntity(UserFirebaseTokenDto a, FirebaseUserToken ent = null)
        {
            if (a == null) return null;
            ent = ent ?? new FirebaseUserToken();
            ent.UserId = a.UserId;
            ent.Token = a.FirebaseToken;
            ent.DeviceId = a.DeviceId;
            ent.LastUpdate = a.LastUpdate;
            return ent;
        }

        public static bool Set(UserFirebaseTokenDto a)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.FirebaseUserTokens.FirstOrDefault(s => s.UserId == a.UserId && s.DeviceId == a.DeviceId);
                if (ent == null)
                {
                    ent = new FirebaseUserToken();
                    context.FirebaseUserTokens.Add(ent);
                }
                a.LastUpdate = DateTime.Now;
                ToEntity(a, ent);
                return context.SaveChanges() > 0;
            }
        }

        public static bool ClearDeviceId(string deviceId)
        {
            using (var context = new LakbayDBEntities())
            {
                var list = context.FirebaseUserTokens.Where(s => s.DeviceId == deviceId);
                var lastUpdate = DateTime.Now;
                foreach (var e in list)
                {
                    e.DeviceId = "";
                    e.LastUpdate = lastUpdate;
                }
                return context.SaveChanges() > 0;
            }
        }

        public static IEnumerable<UserFirebaseTokenDto> Get(int userid)
        {
            using (var context = new LakbayDBEntities())
            {
                return context.FirebaseUserTokens.Where(s => s.UserId == userid).ToList().Select(s => ToDto(s));
            }
        }

        public static IEnumerable<UserFirebaseTokenDto> Get(IEnumerable<int> userids)
        {
            using (var context = new LakbayDBEntities())
            {
                return context.FirebaseUserTokens.Where(s => userids.Contains(s.UserId)).ToList().Select(s => ToDto(s));
            }
        }


        public static void SendPushNotification(IEnumerable<int> toNotifyList, NotificationDto notification)
        {
            var firebaseTokens = UserFirebaseTokenRepo.Get(toNotifyList);
            //var sender = new FirebaseCloudMessagingPush();
            //_ = sender.SendNotificationAsync(firebaseTokens.Select(s => s.FirebaseToken), notification);
        }
    }
}
