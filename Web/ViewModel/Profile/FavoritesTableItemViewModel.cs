using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.ViewModel.Profile
{
    public class FavoritesTableItemViewModel
    {
        public FavoritesTableItemViewModel(UrlHelper helper, string CDNHOST, int userId, PlaceViewModel t)
        {
            urlHelper = helper;
            this.CDNHOST = CDNHOST;
            UserId = userId;
            this.t = t;
        }
        UrlHelper urlHelper { get; set; }
        string CDNHOST { get; set; }
        public int UserId { get; set; }
        PlaceViewModel t { get; set; }

        public string imgSrc => CDNHOST + (t.HomeThumbnail?.Replace(".jpg", "_thumb.jpg") ?? "");
        public bool ShowFavorites => UserId > 0;
        public string FavoritesClass => t?.IsLiked ?? false ? "liked" : "";

        public string ParentName => t.ParentName ?? "";
        public int Id => t.Id;
        public string Title => t.Title ?? "";
        public string Description => t.Description ?? "";
        public string href => $"/Places/Details/{Id}";

    }
}