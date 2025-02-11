using AutoMapper;
using Project.Database.DbContext;
using Project.Models.Enums_;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;
using Infrastructure.Files;
using System.IO;
using System.Drawing;
using Infrastructure.Images;

namespace Project.Repository.Repo
{
    public class UserDetailRepo
    {
        public static UserDetail ToEntity(UserDetailDto details, UserDetail ent = null)
        {
            ent = ent ?? new UserDetail();

            ent.Address = details.Address ?? "";
            ent.Email = details.Email ?? "";
            ent.FullName = details.FullName ?? "";
            ent.IsDeleted = false;
            ent.Mobile = details.Mobile;
            ent.Age = details.Age;
            ent.City = details.City;
            ent.Country = details.Country;
            ent.Gender = (int)((int?)details.Gender ?? (-1));
            ent.UserId = details.UserId;
            ent.ProfilePictureUrl = details.ProfilePictureUrl;

            return ent;
        }
        public static UserDetailDto ToDto(UserDetail ent)
        {
            if (ent == null) return null;
            Gender? gender = ent.Gender == -1 ? null : (Gender?)ent.Gender;
            return new UserDetailDto
            {
                Id = ent.Id,
                Age = ent.Age,
                Address = ent.Address,
                City = ent.City,
                Country = ent.Country,
                Email = ent.Email,
                FullName = ent.FullName,
                Gender = gender,
                IsDeleted = ent.IsDeleted,
                Mobile = ent.Mobile,
                UserId = ent.UserId,
                ProfilePictureUrl = ent.ProfilePictureUrl,
            };
        }

        public static UserDetailDto GetByUserId(int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                return ToDto(context.UserDetails.FirstOrDefault(s => s.UserId == userId));
            }
        }
        public static ImageDto GetProfileImage(int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                return ImagesRepo.ToDto(context.UserDetails.Include(s => s.Image).FirstOrDefault(s => s.UserId == userId)?.Image);
            }
        }


        public static ReturnValue Update(UserDetailDto dto)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.UserDetails.FirstOrDefault(s => s.UserId == dto.UserId);
                ToEntity(dto, ent);
                var success = context.SaveChanges() > 0;
                if(success)
                {
                    return new ReturnValue { Success = success, Message = "Updated successfully" };
                }
                else
                {
                    return new ReturnValue() { Message = "No changes were made." };
                }
              
            }
        }
        public static ReturnValue Update(UserDetailDto dto, HttpPostedFileBase profilePic)
        {
            using (var context = new LakbayDBEntities())
            {
                var success = false;
                var user = context.Users.FirstOrDefault(s => s.Id == dto.UserId);
                if (profilePic != null)
                {
                    var path = UserDetailRepo.GetProfilePictureFolderPath(user.GUID);
                    var uploadProfPic = FileManager.UploadVirtualPath(profilePic, path);
                    if (uploadProfPic.Success)
                    {
                        var systemName = (string)uploadProfPic.ReturnParam;
                        var bitmap = new Bitmap(System.Drawing.Image.FromStream(profilePic.InputStream, true, true));
                        var thumb = ImageResizer.CreateThumbnail(bitmap);
                        FileManager.UploadThumbnailVirtualPath(thumb, path, systemName);
                        var img = new ImageDto
                        {
                            FilePath = path,
                            SystemName = systemName,
                            FileName = profilePic.FileName,
                            FileType = Path.GetExtension(profilePic.FileName),

                        };
                        var oldImage = ImagesRepo.GetUserProfileImage(user.Id);
                        FileManager.DeleteVirtualPath(path, oldImage.SystemName);
                        FileManager.DeleteVirtualPath(path, oldImage.ThumbnailSystemName);
                        UserDetailRepo.ChangeProfilePic(context, dto.UserId, img);
                    }
                }

                var ent = context.UserDetails.FirstOrDefault(s => s.UserId == dto.UserId);
                ToEntity(dto, ent);
                success = context.SaveChanges() > 0;
                if (success)
                {
                    return new ReturnValue { Success = success, Message = "Updated successfully" };
                }
                else
                {
                    return new ReturnValue() { Message = "No changes were made." };
                }

            }
        }

        public static ReturnValue ChangeProfilePictureUrl(int userId, string url)
        {
            using (var context = new LakbayDBEntities())
            {
                var ent = context.UserDetails.FirstOrDefault(s => s.UserId == userId);
                ent.ProfilePictureUrl = url;
                var success = context.SaveChanges() > 0;
                if (success)
                {
                    return new ReturnValue { Success = success, Message = "Updated successfully" };
                }
                else
                {
                    return new ReturnValue() { Message = "Update failed" };
                }

            }
        }

        
     
        public static ReturnValue ChangeProfilePic(int userId, ImageDto image)
        {
            using(var context = new LakbayDBEntities())
            {
                var user = context.UserDetails.Include(s => s.Image).FirstOrDefault(s => s.UserId == userId);
                if(user.Image != null)
                {                   
                    context.Images.Remove(user.Image);
                }
                user.Image = ImagesRepo.ToEntity(image);
                var succ = context.SaveChanges() > 0;
                image.Id = user.Image.Id;
                if (succ)
                {
                    return new ReturnValue { Message = "Image changed", Success = true };
                }
                else
                {
                    return new ReturnValue { Message = "Failed to save" };
                }
            }
        }
        internal static void ChangeProfilePic(LakbayDBEntities context, int userId, ImageDto image)
        {
            var user = context.UserDetails.Include(s => s.Image).FirstOrDefault(s => s.UserId == userId);
            if (user.Image != null)
            {
                context.Images.Remove(user.Image);
            }
            user.Image = ImagesRepo.ToEntity(image);
        }


        public static string GetProfilePictureFolderPath(string guid)
        {
            return $"profilepicture\\{guid}";
        }

    }
}
