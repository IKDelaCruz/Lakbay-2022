using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Models.Enums_;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.ViewModel
{
    public class ProfileViewModel
    {
        internal string countriesListPath { get; set; }
        public ProfileViewModel()
        {
            Dto = new UserDetailDto();
        }
        public ProfileViewModel(string countriesListPath, UserDetailDto dto, string guid)
        {
            this.countriesListPath = countriesListPath;
            Dto = dto;
            Guid = guid;
        }
        public UserDetailDto Dto { get; set; }
        public string FullName { get => Dto.FullName; set => Dto.FullName = value; }
        public Gender? Gender { get => Dto.Gender; set => Dto.Gender = value; }
        public int? Age { get => Dto.Age; set => Dto.Age = value; }
        public string MobileNumber { get => Dto.Mobile; set => Dto.Mobile = value; }
        public string Email { get => Dto.Email; set => Dto.Email = value; }
        public string Country { get => Dto.Country; set => Dto.Country = value; }
        public string City { get => Dto.City; set => Dto.City = value; }
        public int Id { get => Dto.Id; set => Dto.Id = value; }
        public int UserId { get => Dto.UserId; set => Dto.UserId = value; }
        public HttpPostedFileBase UploadProfilePic { get; set; }
        public string Guid { get; set; }
        public List<SelectListItem> GetCountries()
        {
            return GetCountries(countriesListPath, Country);
        }

        public static List<SelectListItem> GetCountries(string countriesListPath, string country = "")
        {
            using (var stream = new FileStream(countriesListPath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var list = JObject.Load(reader);
                return
                    list.Properties().Select(p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Name,
                        Selected = p.Name == country
                    }).ToList();
            }
        }

        public List<SelectListItem> GetCities()
        {
            return GetCities(countriesListPath, Country, City);
        }

        public static List<SelectListItem> GetCities(string countriesListPath, string country, string city = "")
        {
            country = country ?? "";
            using (var stream = new FileStream(countriesListPath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var list = JObject.Load(reader);
                country = country.ToLower();
                var myElement = list.Properties().FirstOrDefault(s => s.Name.ToLower() == country)?.Children().Children();
                var cityList = new List<string>();

                cityList.Add("");


                if (myElement != null)
                {
                    foreach (var key in myElement)
                    {
                        cityList.Add(key.ToString());
                    }
                }
                
                var retlist = cityList.Distinct().Select(p => new SelectListItem
                {
                    Text = p,
                    Value = p,
                    Selected = p == city
                }).ToList();

                if (string.IsNullOrEmpty(city))
                {
                    var item = retlist.FirstOrDefault();
                    if (item != null)
                    {
                        item.Selected = true;
                    }

                }

                return retlist;
            }
        }
    }
}