using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models.Models
{
    public class CountryDto
    {
        public CountryDto()
        {
            Id = 0;
            Name = string.Empty;
            IsDeleted = false;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}