using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models.Models
{
    public class PlaceDto
    {
        public PlaceDto()
        {
            Id = 0;
            CountryId = 0;
            ProvinceId = 0;
            CityId = 0;
            BaranggayId = 0;
            CompleteAddress = string.Empty;
            Title = string.Empty;
            ContactNumber = string.Empty;
            Email = string.Empty;
            Description = string.Empty;
            Directions = string.Empty;
            Url = string.Empty;
            PaymentOption = PaymentOptionEnums.Card;
            Type = CategoryTypeEnum.Accommodation;
            IsDeleted = false;
            GUID = string.Empty;
        }

        public int Id { get; set; }
        public int ChildId { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public int BaranggayId { get; set; }
        public string CompleteAddress { get; set; }
        public string Title { get; set; }
        public string HomeThumbnail { get; set; }
        public string HomeThumbnailFullPath { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string DescriptionHTML { get; set; }
        public string Directions { get; set; }
        public string Url { get; set; }
        public string ShopUrl { get; set; }
        public PaymentOptionEnums PaymentOption { get; set; }
        public CategoryTypeEnum Type { get; set; }
        public bool IsDeleted { get; set; }
        public string HomeSlug { get; set; }
        public string Tagline { get; set; }

        public decimal DistanceFromManila { get; set; }
        public int TotalPlaces { get; set; }
        public int DisplayPriority { get; set; }
        public DateTime PublishedDate { get; set; }
        public string PublishedBy { get; set; }

        public string ContactPerson { get; set; }
        public string MapsLongtitude { get; set; }
        public string MapsLatitude { get; set; }
        public string HeaderVideoURL { get; set; }
        public string ParentName { get; set; }
        public int ParentId { get; set; }
        public string GUID { get; set; }


        public double MeterDistanceFromUser { get; set; }
    }
}
