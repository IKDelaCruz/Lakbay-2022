using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.ViewModel;

namespace Web.Areas.Mobile.ViewModels
{
    public class FavoritesViewModel
    {
        public FavoritesViewModel(UrlHelper helper, string CDNHOST, int userId, PlaceViewModel t)
        {
            urlHelper = helper;
            this.CDNHOST = CDNHOST;
            UserId = userId;
            this.t = t;
        }
        string CDNHOST { get; set; }
        UrlHelper urlHelper { get; set; }
        public PlaceViewModel t { get; set; }
        public int UserId { get; set; }
        public string favoritesClass => t?.IsLiked ?? false ? "liked" : "";
        public string url => string.Format("goto/details?id={0}&userid={1}", t.Id, UserId);
        public bool showFavorites => UserId > 0;
        public string favoritesDisplay => showFavorites ? "" : "none";

        public string parent
        {
            get
            {
                if (string.IsNullOrEmpty(t.ParentName))
                {
                    return "";
                }
                else
                {
                    return $"<small>{t.ParentName}</small>";
                }
            }
        }

        
        public int Id => t.Id;
        public string imgSrc => CDNHOST + (t.HomeThumbnail?.Replace(".jpg", "_thumb.jpg") ?? "");
        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(t.Description))
                {
                    return "";
                }
                else if(t.Description.Length > 200)
                {
                    return $"{t.Description.Substring(0, 200)}...";
                }
                else
                {
                    return t.Description;
                }
            }

        }
    }
}