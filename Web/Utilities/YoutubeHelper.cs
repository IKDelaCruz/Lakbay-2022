using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Project.Models.Enums;

namespace Web.Utilities
{
    public class YoutubeHelper
    {
        public static void SetupYoutube(PlaceImagesDto p)
        {
            if (p.Type == PlaceImagesType.YoutubeURL)
            {
                // if p.filename has youtube url, move to url to p.youtubeurl
                if ((p.Filename?.Contains("https") ?? false) && string.IsNullOrEmpty(p.YoutubeUrl))
                {
                    p.YoutubeUrl = p.Filename;
                    p.Filename = "";
                }
                else if ((p.Filename?.Contains("https") ?? false) && !string.IsNullOrEmpty(p.YoutubeUrl))
                {
                    p.Filename = "";
                }
                p.YouTubeId = Infrastructure.YoutubeHelper.GetIdFromUrl(p.YoutubeUrl);
                p.YoutubeThumbnail = Infrastructure.YoutubeHelper.GetThumbnailFromId(p.YouTubeId);

            }
        }
    }
}