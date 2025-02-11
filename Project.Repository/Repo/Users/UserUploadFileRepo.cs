using Project.Database.DbContext;
using Project.Models.Models;
using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Repository.Repo.Users
{
    public static class UserUploadFileRepo
    {
        private static UserUploadFileDto ToDto(this UserUploadedFile rec)
        {
            if (rec == null)
                return null;

            return new UserUploadFileDto()
            {
                Id = rec.Id,
                UserId = rec.UserId,
                Path = rec.Path,
                Type = (UserUploadFileTypeEnum)rec.Type,
                Filename = rec.Filename,
                FileExtension = rec.FileExtension,
                FileSize = rec.FileSize,
                UploadBy = rec.UploadBy,
                DateUploaded = rec.DateUploaded,
                LastDateModified = rec.LastDateModified,
                Remarks = rec.Remarks,
                IsDeleted = rec.IsDeleted,
                Guid = rec.Guid
            };
        }


        public static IEnumerable<UserUploadFileDto> GetList(int userId)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                return context.UserUploadedFiles.Where(a => a.UserId == userId && !a.IsDeleted).ToList().Select(b => b.ToDto());
            }
        }

        public static UserUploadFileDto Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var file = context.UserUploadedFiles.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                return file.ToDto();
            }
        }
        public static UserUploadFileDto Get(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var file = context.UserUploadedFiles.FirstOrDefault(a => a.Guid == guid && !a.IsDeleted);
                return file.ToDto();
            }
        }

        public static ReturnValue Add(IEnumerable<UserUploadFileDto> dtos)
        {
            var result = new ReturnValue();

            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                foreach (var dto in dtos)
                {
                    SaveToDB(context, dto);
                }

                var saveChanges = context.SaveChanges();
                if (saveChanges > 0)
                {
                    result.Message = "File(s) successfully uploaded.";
                    result.Success = true;
                }
                else
                {
                    result.Message = "Unable to upload file(s).";
                }
            }

            return result;
        }

        public static ReturnValue Add(UserUploadFileDto dto)
        {
            var result = new ReturnValue();

            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                SaveToDB(context, dto);

                var saveChanges = context.SaveChanges();
                if (saveChanges > 0)
                {
                    result.Message = "File successfully uploaded.";
                    result.Success = true;
                }
                else
                {
                    result.Message = "Unable to upload file.";
                }
            }

            return result;
        }

        public static ReturnValue Update(UserUploadFileDto dto)
        {
            var result = new ReturnValue();

            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.UserUploadedFiles.FirstOrDefault(a => a.Id == dto.Id);

                if (record != null)
                {
                    record.Update(dto);

                    var saveChanges = context.SaveChanges();
                    if (saveChanges > 0)
                    {
                        result.Message = "Changes successfully saved.";
                        result.Success = true;
                    }
                    else if (saveChanges == 0)
                    {
                        result.Message = "Nothing to save.";
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = "Unable to save changes.";
                    }
                }
                else
                {
                    result.Message = "Unable to access file.";
                }
            }

            return result;
        }
        public static ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.UserUploadedFiles.FirstOrDefault(a => a.Id == id);

                if (record != null)
                {
                    var dto = record.ToDto();
                    dto.IsDeleted = true;

                    record.Update(dto);
                    record.IsDeleted = true;
                    var saveChanges = context.SaveChanges();
                    if (saveChanges > 0)
                    {
                        result.Message = "File successfully deleted.";
                        result.Success = true;
                    }
                    else if (saveChanges == 0)
                    {
                        result.Message = "Nothing to delete.";
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = "Unable to delete this file.";
                    }
                }
                else
                {
                    result.Message = "Unable to access file.";
                }
            }

            return result;
        }
        public static ReturnValue Delete(string guid)
        {
            var result = new ReturnValue();

            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.UserUploadedFiles.FirstOrDefault(a => a.Guid == guid);

                if (record != null)
                {
                    var dto = record.ToDto();
                    dto.IsDeleted = true;

                    record.Update(dto);
                    record.IsDeleted = true;
                    var saveChanges = context.SaveChanges();
                    if (saveChanges > 0)
                    {
                        result.Message = "File successfully deleted.";
                        result.Success = true;
                    }
                    else if (saveChanges == 0)
                    {
                        result.Message = "Nothing to delete.";
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = "Unable to delete this file.";
                    }
                }
                else
                {
                    result.Message = "Unable to access file.";
                }
            }

            return result;
        }

        private static void SaveToDB(LakbayDBEntities context, UserUploadFileDto dto)
        {
            var file = new UserUploadedFile()
            {
                Id = dto.Id,
                UserId = dto.UserId,
                Path = dto.Path,
                Type = (int)dto.Type,
                Filename = dto.Filename,
                FileExtension = dto.FileExtension,
                FileSize = dto.FileSize,
                UploadBy = dto.UploadBy,
                DateUploaded = dto.DateUploaded,
                LastDateModified = dto.LastDateModified,
                Remarks = dto.Remarks,
                IsDeleted = dto.IsDeleted,
                Guid = Guid.NewGuid().ToString()
            };

            context.UserUploadedFiles.Add(file);
        }

        private static void Update(this UserUploadedFile record, UserUploadFileDto dto)
        {
            if (record != null)
            {
                record.Path = dto.Path;
                record.Type = (int)dto.Type;
                record.Filename = dto.Filename;
                record.FileExtension = dto.FileExtension;
                record.FileSize = dto.FileSize;
                record.UploadBy = dto.UploadBy;
                record.DateUploaded = dto.DateUploaded;
                record.LastDateModified = dto.LastDateModified;
                record.Remarks = dto.Remarks;
            };
        }
    }
}
