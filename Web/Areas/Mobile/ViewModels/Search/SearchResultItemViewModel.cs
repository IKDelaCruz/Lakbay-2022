using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utilities;

namespace Web.Areas.Mobile.ViewModels
{
    public class SearchResultItemViewModel
    {
        public SearchResultItemViewModel(UrlHelper helper, string CDNHOST, int userId, PlaceDto t, bool showDistance)
        {
            urlHelper = helper;
            this.CDNHOST = CDNHOST;
            UserId = userId;
            this.t = t;
            ShowDistance = showDistance;
        }
        string CDNHOST { get; set; }
        UrlHelper urlHelper { get; set; }
        public int UserId { get; set; }
        public PlaceDto t { get; set; }

        public string url => urlHelper.Action("destinationdetails", "goto", new { area = "mobile", id = t.Id, userid = UserId });

        public string backgroundImg => CDNHOST + (t.HomeThumbnail?.Replace(".jpg", "_thumb.jpg") ?? "");

        public string ParentName => t.ParentName.ToUpper();

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(t.Description))
                {
                    return "";
                }
                else
                {
                    return $"<span>{t.Description.TruncateLongString(80)}</span>";
                }
            }
        }


        public bool ShowDistance { get; set; }
        private static string GetApproximateDistance(double distance)
        {
            var num = Convert.ToInt32(distance);
            var firstDigit = 0;
            var zeroes = 0;

            firstDigit = num;
            while (firstDigit >= 10)
            {
                zeroes++;
                firstDigit /= 10;
            }


            if (num > 1000)
            {
                zeroes -= 3;
                return firstDigit.ToString() + new String('0', zeroes) + ".00 km";
            }
            else
            {
                return firstDigit.ToString() + new String('0', zeroes) + ".00 m";
            }
        }

        public string DistanceFromUserString
        {
            get
            {
                if(t.MeterDistanceFromUser == 0 
                    || !ShowDistance
                    || t.MapsLatitude == null 
                    || t.MapsLongtitude == null
                    )
                   // || t.MeterDistanceFromUser > 100000)//100km
                {
                    return "";
                }
                return $"{GetApproximateDistance(t.MeterDistanceFromUser)}";

            }
        }
    }

}