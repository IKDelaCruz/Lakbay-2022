using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.ViewModel.Profile
{
    public class HistoryTableItemViewModel
    {
        public HistoryTableItemViewModel(UrlHelper helper, string CDNHOST, int userId, UserPlaceVisitModel t)
        {
            urlHelper = helper;
            this.CDNHOST = CDNHOST;
            UserId = userId;
            this.t = t;
        }
        UrlHelper urlHelper { get; set; } 
        string CDNHOST { get; set; }
        public int UserId { get; set; }
        public UserPlaceVisitModel t { get; set; }

        public string imgSrc => CDNHOST + (t.Place.HomeThumbnail?.Replace(".jpg", "_thumb.jpg") ?? "");

        public string DateVisitLongDate => t.dto.DateVisit.ToLongDateString();
        public string DateVisitShortTime => t.dto.DateVisit.ToShortTimeString();
        public string ParentName => t.Place.ParentName;

        public string Remarks => t.dto.Remarks ?? "";
    }
}