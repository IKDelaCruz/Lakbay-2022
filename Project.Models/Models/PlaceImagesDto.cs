using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models.Models
{
    public class PlaceImagesDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string Filename { get; set; }
        public string ImageText { get; set; }
        public string YouTubeId { get; set; }
        public PlaceImagesType Type { get; set; }

        public string YoutubeUrl { get; set; }
        public string YoutubeThumbnail { get; set; }
    }
}
