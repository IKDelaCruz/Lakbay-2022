using Project.Models;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utilities;

namespace Web.ViewModel
{
    public class CategoriesDetailViewModel
    {
        public CategoriesDetailViewModel(UrlHelper helper, PlaceDto place, string CDNHOST, int userId, IEnumerable<FavoritesDto> list,
            string userLongitude, string userLatitude)
        {
            urlHelper = helper;
            t = place;
            this.CDNHOST = CDNHOST;
            UserId = userId;
            IsLiked = list.Any(s => t.Id == s.Place.Id);
            UserLongitude = userLongitude;
            UserLatitude = userLatitude;
        }
        private string UserLatitude { get; set; }
        private string UserLongitude { get; set; }
        UrlHelper urlHelper { get; set; }
        PlaceDto t { get; set; }
        PlaceViewModel vm 
        { 
            get 
            {
                if (t == null) return null;
                return new PlaceViewModel(t, null, UserLongitude, UserLatitude);
            }
        }
        #region favorites control
        public bool IsLiked { get; set; }
        public bool ShowFavorites => UserId > 0;
        public string FavoritesClass => IsLiked ? "liked" : "";
        #endregion

        string CDNHOST { get; set; }
        public int Id => t.Id;
        public int UserId { get; set; }
        public string Distance => vm?.DistanceFromUserString;
        public string EventUrl => urlHelper.Action("eventdetails", "goto", new { area = "mobile", id = t.Id, userId = UserId });
        public string imgsrc => CDNHOST + t.HomeThumbnail?.Replace(".jpg", "_thumb.jpg") ?? "";
        public string ParentName => t.ParentName ?? "";
        public string ParentNameUpper => t.ParentName?.ToUpper() ?? "";
        public string PublishedDate => t.PublishedDate.ToString("MMMM dd");
        public bool HasDescription => !string.IsNullOrEmpty(t.Description);
        public string Description200 => HasDescription ? t.Description.TruncateLongString(200) : "";

        public bool IsEvent => t.Type == Enums.CategoryTypeEnum.NewsEvents;

        public string Title => t.Title;
        public string PlaceUrl => urlHelper.Action("details", "goto", new { area = "mobile", id = t.Id, userId = UserId });
        public string Description100 => HasDescription ? t.Description.TruncateLongString(100) : "";
        public string CompleteAddress => t.CompleteAddress?.ToUpper();

        public bool HasUrl => !string.IsNullOrEmpty(t.Url);
        public string WebsiteUrl => t.Url;
        public string ShopUrl => t.ShopUrl;
        public bool HasShopUrl => !string.IsNullOrEmpty(t.ShopUrl);

        public bool HasEmail => !string.IsNullOrEmpty(t.Email);
        public string Email => HasEmail ? t.Email : "";
        public string EmailTruncate => HasEmail ? t.Email.TruncateEmail(30) : "";


        public bool HasContactNumber => !string.IsNullOrEmpty(t.ContactNumber);
        public List<string> ContactNumbers => HasContactNumber ? GetContactNumbers() : new List<string>();
        public int NumberContacts => GetNumberContacts();

        public int GetNumberContacts()
        {
            if (!HasContactNumber) return 0;
            if (t.ContactNumber?.IndexOf(',') > 0)
            {
                var nos = t.ContactNumber.Split(',');
                return nos.Length;
            }
            else
            {
                return 1;
            }
        }
        public List<string> GetContactNumbers()
        {
            if(t.ContactNumber?.IndexOf(',') > 0)
            {
                var nos = t.ContactNumber.Split(',');
                return nos.Where(s => s.Length > 0).ToList();
            }
            else
            {
                var list = new List<string>();
                list.Add(t.ContactNumber);
                return list;
            }
        
        }

    }
}