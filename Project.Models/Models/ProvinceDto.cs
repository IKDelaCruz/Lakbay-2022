using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models.Models
{
    public class ProvinceDto
    {
        public ProvinceDto()
        {
            Id = 0;
            CountryId = 0;
            CountryName = string.Empty;
            Name = string.Empty;
            IsDeleted = false;
        }

        public int Id { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}