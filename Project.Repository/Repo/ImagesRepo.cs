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
    public class ImagesRepo
    {
        public static ImageDto ToDto(Image ent)
        {
            if (ent == null) return null;
            return new ImageDto
            {
                Id = ent.Id,
                Deleted = ent.Deleted,
                FilePath = ent.Path,
                SystemName = ent.SystemName,
                FileName = ent.FileName,
                FileType = ent.FileType
            };
        }
        public static Image ToEntity(ImageDto dto, Image ent = null)
        {
            ent = ent ?? new Image();
            ent.SystemName = dto.SystemName;
            ent.Deleted = dto.Deleted;
            ent.Path = dto.FilePath;
            ent.FileType = dto.FileType;
            ent.FileName = dto.FileName;
            return ent;
        }

        public static ImageDto Get(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                var rec = context.Images.FirstOrDefault(s => s.Id == id && !s.Deleted);
                return ToDto(rec);
            }
        }

        public static ImageDto GetUserProfileImage(int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                var rec = context.UserDetails.Include(s => s.Image).FirstOrDefault(s => s.UserId == userId);

                return ToDto(rec.Image);
            }
        }

        public static bool Add(ImageDto dto)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = ToEntity(dto);
                context.Images.Add(ent);
                var succ = context.SaveChanges() > 0;
                dto.Id = ent.Id;
                return succ;
            }
        }

        internal static ICollection<Image> AddFiles(List<ImageDto> FileDtos, ICollection<Image> FileEntities = null)
        {
            FileEntities = FileEntities ?? new List<Image>();
            foreach (var dto in FileDtos)
            {
                FileEntities.Add(ToEntity(dto));
            }
            return FileEntities;
        }

        internal static ICollection<Image> RemoveFiles(List<int> toDelete, ICollection<Image> FileEntities)
        {
            var temp = FileEntities.ToList();
            foreach (var id in toDelete)
            {
                var rec = temp.FirstOrDefault(s => s.Id == id);
                if (rec != null)
                {
                    rec.Deleted = true;
                    FileEntities.Remove(rec);
                }
            }
            return FileEntities;
        }
    }
}
