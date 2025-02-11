using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models.Models
{
    public class CityDto
    {
        public CityDto()
        {
            Id = 0;
            ProvinceId = 0;
            ProvinceName = string.Empty;
            Name = string.Empty;
            IsDeleted = false;
        }

        public int Id { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}