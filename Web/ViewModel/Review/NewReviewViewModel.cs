using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModel.Review
{
    public class NewReviewViewModel
    {
        public int CurrentUserid { get; set; }
        public NewReviewViewModel()
        {
            Dto = new ReviewDto();
        }
        public ReviewDto Dto { get; set; }
        public bool IsEditor => Dto.UserId == CurrentUserid;
        public int Id => Dto.Id;
        public int PlaceId => Dto.PlaceId;
        public int ReviewStars => Dto.ReviewStars;

    }
}