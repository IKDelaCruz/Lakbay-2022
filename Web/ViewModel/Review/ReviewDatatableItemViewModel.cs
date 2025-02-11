using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.ViewModel.Review
{
    public class ReviewDatatableItemViewModel
    {
        public ReviewDatatableItemViewModel(UrlHelper helper, string CDNHOST, int userId, ReviewDto t)
        {
            urlHelper = helper;
            this.CDNHOST = CDNHOST;
            UserId = userId;
            this.t = t;
        }
        protected UrlHelper urlHelper { get; set; }
        protected string CDNHOST { get; set; }
        public int UserId { get; set; }
        public ReviewDto t { get; set; }

        public virtual string ReviewStars
        {
            get
            {
                var text = "";
                for (int i = 0; i < t.ReviewStars; i++)
                {
                    text += "<i class='icon_star voted'></i>";
                }
                return text;
            }
        }
        public string profileImgSrc => urlHelper.Action("ProfilePictureById", "Profile", new { id = t.UserId, isThumbnail = true , area = "" });
        public string CreatedDateLongDateString => t.CreatedDate.ToLongDateString();

        public string Info => $"{t.Username} - {CreatedDateLongDateString}";

        public string ShowReviewControls => t.UserId == UserId ? "" : "none";
    }
}
