using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models.Models.UserModels
{
    public class UserUploadFileDto
    {
        public UserUploadFileDto()
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
    }
}
