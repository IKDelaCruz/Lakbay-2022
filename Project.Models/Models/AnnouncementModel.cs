using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models
{
    public class AnnouncementModel
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string MobileImageURL { get; set; }
        public DateTime DateUploaded { get; set; }
        public int UploadBy { get; set; }
        public int Status { get; set; }
        public string Details { get; set; }
        public int LinkId { get; set; }
        public bool IsActive { get; set; }
    }
}
