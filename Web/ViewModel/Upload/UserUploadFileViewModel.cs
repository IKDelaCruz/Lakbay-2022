using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Project.Models.Enums;

namespace Web.ViewModel
{
    public class UserUploadFileViewModel
    {
        public UserUploadFileViewModel()
        {
            Id = 0;
            UserId = 0;
            Path = string.Empty;
            Type = UserUploadFileTypeEnum.Other;
            Filename = string.Empty;
            FileExtension = string.Empty;
            FileSize = string.Empty;
            UploadBy = 0;
            DateUploaded = DateTime.Now;
            LastDateModified = DateTime.Now;
            Remarks = string.Empty;
            IsDeleted = false;
            Guid = string.Empty;
        }

        public string Guid { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Path { get; set; }
        public UserUploadFileTypeEnum Type { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public string FileSize { get; set; }
        public int UploadBy { get; set; }
        public DateTime DateUploaded { get; set; }
        public DateTime LastDateModified { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public string imgUrl { get; set; }

        public HttpPostedFileBase File { get; set; }

        public IEnumerable<SelectListItem> Types
        {
            get
            {
                return Enum.GetValues(typeof(UserUploadFileTypeEnum)).Cast<UserUploadFileTypeEnum>().Select(a => new SelectListItem()
                {
                    Text = a.ToString().Replace("_", " "),
                    Value = ((int)a).ToString(),
                });
            }
        }

        public UserUploadFileDto ToDto()
        {
            return new UserUploadFileDto()
            {
                Id = Id,
                UserId = UserId,
                Path = Path,
                Type = Type,
                Filename = Filename,
                FileExtension = FileExtension,
                FileSize = !string.IsNullOrEmpty(FileSize) ? FileSize : "",
                UploadBy = UploadBy,
                DateUploaded = DateUploaded,
                LastDateModified = LastDateModified,
                Remarks = Remarks,
                IsDeleted = IsDeleted,
                Guid = Guid
            };
        }

        public UserUploadFileViewModel ToModel(UserUploadFileDto dto)
        {
            return new UserUploadFileViewModel()
            {
                Id = dto.Id,
                UserId = dto.UserId,
                Path = dto.Path,
                Type = dto.Type,
                Filename = dto.Filename,
                FileExtension = dto.FileExtension,
                FileSize = dto.FileSize,
                UploadBy = dto.UploadBy,
                DateUploaded = dto.DateUploaded,
                LastDateModified = dto.LastDateModified,
                Remarks = dto.Remarks,
                IsDeleted = dto.IsDeleted,
                Guid = dto.Guid
            };
        }
    }
}