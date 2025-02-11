using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure
{
    public class YoutubeHelper
    {
        public static string GetIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var videoId = string.Empty;
            if (query.AllKeys.Contains("v"))
            {
                videoId = query["v"];
            }
            else
            {
                videoId = uri.Segments.Last();
            }
            return videoId;
        }

        public static string CreateUrl(string id)
        {
            if (string.IsNullOrEmpty(id.Trim()))
            {
                return "";
            }
            return "https://www.youtube.com/watch?v=" + id;
        }

        public static string GetThumbnailFromId(string id)
        {
            if (string.IsNullOrEmpty(id.Trim()))
            {
                return "";
            }
            return $"https://img.youtube.com/vi/{id}/mqdefault.jpg";
        }
    }
}
