using Project.Database.DbContext;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repository.Repo
{
    public class AnnouncementRepo
    {
        public static AnnouncementModel ToDto(Announcement item)
        {
            if (item == null)
                return null;

            return new AnnouncementModel()
            {
                Id = item.Id,
                ImageURL = item.ImageURL,
                DateUploaded = item.DateUploaded,
                UploadBy = item.UploadBy,
                Status = item.Status,
                Details = item.Details,
                LinkId = item.LinkId.Value,
                IsActive = item.IsActive,
                MobileImageURL = item.MobileImageURL
            };
        }

        public static AnnouncementModel Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                return ToDto(context.Announcements.FirstOrDefault(a => a.Id == id));
            }
        }

        public static IEnumerable<AnnouncementModel> GetList(bool getActiveOnly = true)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Announcements.Where(a => a != null);

                if (getActiveOnly)
                    records = records.Where(a => a.IsActive);

                return records.ToList().Select(a => ToDto(a));
            }
        }

        public static ReturnValue Add(AnnouncementModel dto)
        {
            var result = new ReturnValue("Unable to add announcement at this time. Please try again later.");
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = new Announcement()
                {
                    Id = dto.Id,
                    ImageURL = dto.ImageURL,
                    DateUploaded = dto.DateUploaded,
                    UploadBy = dto.UploadBy,
                    Status = dto.Status,
                    Details = dto.Details,
                    LinkId = dto.LinkId,
                    IsActive = dto.IsActive,
                };
                context.Announcements.Add(item);

                if (context.SaveChanges() > 0)
                {
                    result.Success = true;
                    result.Message = "Announcement successfully added.";
                }
            }

            return result;
        }

        public static ReturnValue Update(AnnouncementModel dto)
        {
            var result = new ReturnValue("Unable to update announcement details at this time. Please try again later.");
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = context.Announcements.FirstOrDefault(a => a.Id == dto.Id);

                if (item != null)
                {
                    item.Id = dto.Id;
                    item.ImageURL = dto.ImageURL;
                    item.DateUploaded = dto.DateUploaded;
                    item.UploadBy = dto.UploadBy;
                    item.Status = dto.Status;
                    item.Details = dto.Details;
                    item.LinkId = dto.LinkId;
                    item.IsActive = dto.IsActive;

                    if (context.SaveChanges() > 0)
                    {
                        result.Success = true;
                        result.Message = "Announcement successfully updated.";
                    }
                }
            }

            return result;
        }
    }
}
