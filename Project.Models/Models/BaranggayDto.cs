using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models.Models
{
    public class BaranggayDto
    {
        public BaranggayDto()
        {
            Id = 0;
            CityId = 0;
            CountryId = 0;
            ProvinceId = 0;
            ProvinceName = string.Empty;
            CityName = string.Empty;
            Name = string.Empty;
            Longitude = 0;
            Latitude = 0;
            ZipCode = 0;
            IsDeleted = false;
        }

        public int Id { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int ZipCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}