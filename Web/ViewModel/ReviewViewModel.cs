using Project.Models.Models;
using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModel
{
    public class ReviewViewModel
    {
        public ReviewViewModel(int placeId, int userId, int limit)
        {
            CDNHOST = System.Configuration.ConfigurationManager.AppSettings["CDNHOST"];
            var mun = ReviewRepo.GetList(placeId, limit);
            var user = UserRepo.Get(userId);

            PlaceReviews = mun.ToList();
            ParentId = placeId;
            UserId = userId;
            UserGuid = user?.GUID ?? "";
            TotalReviewsCount = ReviewRepo.GetReviewsCount(placeId);
            //UserReview = ReviewRepo.GetReview(userId, placeId);

            UserReview = null;
        }
        public string CDNHOST { get; set; }
        public int ParentId { get; set; }
        public int UserId { get; set; }
        public string UserGuid { get; set; }
        public List<ReviewDto> PlaceReviews { get; set; }
        public int TotalReviewsCount { get; set; }
        public ReviewDto UserReview { get; set; }



        public string UserReviewText => UserReview?.ReviewText ?? "";
    }
}