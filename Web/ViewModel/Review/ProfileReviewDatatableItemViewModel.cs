using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.ViewModel.Review
{
    public class ProfileReviewDatatableItemViewModel : ReviewDatatableItemViewModel
    {
        public PlaceDto p { get; set; }
        public ProfileReviewDatatableItemViewModel(UrlHelper helper, string CDNHOST, int userId, ReviewDto t, PlaceDto p) : base(helper, CDNHOST, userId, t)
        {
            this.p = p;
        }
        public override string ReviewStars
        {
            get
            {
                var text = "";
                for (int i = 0; i < t.ReviewStars; i++)
                {
                    text += "<i class='fa fa-star' style='color: #3dc13d;'></i>";
                }
                return text;
            }
        }

        public string PlaceUrl => urlHelper.Action("details", "goto", new { area = "mobile", id = p.Id, userId = UserId });
        public string createdDate => t.CreatedDate.ToLongDateString();
        public string placeImgSrc => (CDNHOST + (p.HomeThumbnail.Replace(".jpg", "_thumb.jpg")));
    }
}
