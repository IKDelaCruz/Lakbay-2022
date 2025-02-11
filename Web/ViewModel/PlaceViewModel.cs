using AutoMapper;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Project.Models.Enums;

namespace Web.ViewModel
{
    public class PlaceDetailViewModel
    {
        public List<PlaceImagesDto> Images { get; set; }
        public PlaceDetailViewModel()
        {
            CDNHOST = System.Configuration.ConfigurationManager.AppSettings["CDNHOST"];
        }
        public bool IsFavorite
        {
            get
            {
                return Project.Repository.Repo.FavoritesRepo.IsFavorite(Place.Id, UserId);
            }
        }
        public string CDNHOST { get; set; }
        public PlaceDto Place { get; set; }
        public int UserId { get; set; }
        public ReviewViewModel ReviewViewModel { get; set; }
    }

    public class PlaceHomeViewModel : BaseViewModel
    {
        public PlaceHomeViewModel()
        {
            CDNHOST = System.Configuration.ConfigurationManager.AppSettings["CDNHOST"];
        }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string CDNHOST { get; set; }
        public List<ReviewDto> PlaceReviews { get; set; }
        public List<PlaceDto> Places { get; set; }
        public CategoryTypeEnum Type { get; set; }
        public int UserId { get; set; }
        public string SearchKeyword { get; set; }

        private List<PlaceViewModel> _PlacesViewModel { get; set; }
        public IEnumerable<PlaceDto> PlacesViewModel
        {
            get => _PlacesViewModel != null ? _PlacesViewModel.Select(s => s as PlaceDto) : Places;
            set
            {
                _PlacesViewModel = value.Select(s => s as PlaceViewModel).ToList();
            }
        }

        public string UserLongitude { get; set; }
        public string UserLatitude { get; set; }
    }

    public class PlaceViewModel : PlaceDto
    {      
        public PlaceViewModel(PlaceDto place, IEnumerable<FavoritesDto> list, string userLongitude = "", string userLatitude = "")
        {        
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PlaceDto, PlaceViewModel>();
            });

            IMapper mapper = config.CreateMapper();
            var dest = mapper.Map(place,this);

            if(list!=null)
                IsLiked = list.Any(s => Id == s.Place.Id);

            //ShowDistance = showDistance;
            UserLongitude = userLongitude;
            UserLatitude = userLatitude;
        }
        public bool IsLiked { get; set; }
        public string UserLongitude { get; set; }
        public string UserLatitude { get; set; }
        private static string GetApproximateDistance(double distance)
        {
            var num = Convert.ToInt32(distance);
            var numString = num.ToString();
            var digits = numString.Length;

            if (digits == 1)
            {
                return numString + "m";
            }

            var temp = numString.Substring(0, 2);
            var zeroes = digits - 2;

            if (num > 1000)
            {
                zeroes -= 3;
                if (zeroes == 0)
                {
                    return temp + "km";
                }
                else if (zeroes < 0)
                {
                    return temp.Insert(1, ".") + "km";
                }
                else
                {
                    return temp + new String('0', zeroes) + ".00 km";
                }
            }
            else
            {
                return temp + new String('0', zeroes) + ".00 m";
            }
        }

        public string DistanceFromUserString
        {
            get
            {
                if (MeterDistanceFromUser == 0
                    || string.IsNullOrEmpty(UserLatitude)
                    || string.IsNullOrEmpty(UserLongitude)
                    || MapsLatitude == null
                    || MapsLongtitude == null
                   // || MeterDistanceFromUser > 1000000//1000km
                    )
                //
                {
                    return "";
                }
                return $"{GetApproximateDistance(MeterDistanceFromUser)}";

            }
        }
    }
}