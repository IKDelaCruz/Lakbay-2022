using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string SystemName { get; set; }
        public string ThumbnailSystemName
        { 
            get
            {
                var extension = Path.GetExtension(SystemName);
                return SystemName.Replace(extension, "_thumbnail.png");
            } 
        }
        public bool Deleted { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

    }
}
