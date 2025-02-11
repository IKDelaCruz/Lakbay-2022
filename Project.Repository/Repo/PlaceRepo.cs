using Infrastructure;
using Project.Database.DbContext;
using Project.Models;

using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Repository.Repo
{
    public class PlaceRepo
    { 
        public static void ComputeDistanceFromUser(IEnumerable<PlaceDto> list, string userLongitude, string userLatitude)
        {
            double _userLongitude = 0;
            double _userLatitude = 0;
            Double.TryParse(userLongitude, out _userLongitude);
            Double.TryParse(userLatitude, out _userLatitude);
            var coord = new GeoCoordinate(_userLatitude, _userLongitude);

            foreach (var s in list)
            {
                double placeLongitude = 0;
                double placeLatitude = 0;
                Double.TryParse(s.MapsLongtitude, out placeLongitude);
                Double.TryParse(s.MapsLatitude, out placeLatitude);
                s.MeterDistanceFromUser = new GeoCoordinate(placeLatitude, placeLongitude).GetDistanceTo(coord);
            }

        }
        public static IEnumerable<PlaceDto> FilterClosest(IEnumerable<PlaceDto> list, string userLongitude, string userLatitude)
        {
            try
            {
                ComputeDistanceFromUser(list, userLongitude, userLatitude);

                return list.OrderBy(s => s.MeterDistanceFromUser).Select(s => s);
            }
            catch(Exception ex)
            {
                //todo remove
               var qwe = new List<PlaceDto>();
                var dto = Get(1);
                dto.Title = ex.Message;
                dto.Description = userLongitude + " " + userLatitude;
                
                qwe.Add(dto);
                return qwe;
            }

        }

        public static void AddGuids()
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var places = context.Places.Where(s => string.IsNullOrEmpty(s.GUID));

                foreach (var place in places)
                {
                    place.GUID = Guid.NewGuid().ToString();
                }
                context.SaveChanges();
            }
        }

        public static PlaceDto Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.Places.FirstOrDefault(h => h.Id == id);

                if (record == null)
                    return null;
                if (string.IsNullOrEmpty(record.MapsLatitude))
                {
                    //set default pin to provincial capitol if there is no GPS data
                    record.MapsLatitude = "13.405567540210633";
                    record.MapsLongtitude = "121.17986818927146";
                }

                var baranggayList = GetBaranggayList(record.BaranggayId ?? 0, context);
                return ToDto(record, baranggayList);
            }
        }
        public static PlaceDto GetByHomeSlug(string homeSlug)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.Places.FirstOrDefault(h => h.HomeSlug.ToLower() == homeSlug.ToLower());

                if (record == null)
                    return null;

                if (string.IsNullOrEmpty(record.MapsLatitude))
                {
                    //set default pin to provincial capitol if there is no GPS data
                    record.MapsLatitude = "13.405567540210633";
                    record.MapsLongtitude = "121.17986818927146";
                }
                var baranggayList = GetBaranggayList(record.BaranggayId ?? 0, context);
                return ToDto(record, baranggayList);
            }
        }
        public static PlaceDto Get(string guid)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = context.Places.FirstOrDefault(a => a.GUID.ToLower() == guid.ToLower());
                if (item == null) return null;
                var baranggayList = GetBaranggayList(item.BaranggayId ?? 0, context);
                var record = ToDto(item,baranggayList);

                if (record == null)
                    return null;

                return record;
            }
        }

        public static IEnumerable<PlaceDto> GetList()
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.Places.Include(s => s.Baranggay).Include(s => s.Baranggay.City).Where(a => !a.IsDeleted).ToList();
                var baranggayList = GetBaranggayList(list.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return list.Select(a => ToDto(a, baranggayList)).ToList();
            }
        }
        public static IEnumerable<PlaceDto> GetList(string types, string param)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var places = context.Places.Include(s => s.Baranggay).Include(s => s.Baranggay.City).Where(a => !a.IsDeleted).ToList();
                List<int> placeTypes = new List<int>();

                if (!string.IsNullOrEmpty(types))
                {
                    var t = types.Split('|').Where(a => !string.IsNullOrEmpty(a)).Select(b => Convert.ToInt32(b));
                    placeTypes.AddRange(t);

                    places = places.Where(a => placeTypes.Any(b => b == a.Type)).ToList();
                }

                if (!string.IsNullOrEmpty(param))
                {
                    places = places.Where(a => a.Title.ToLower().Contains(param.ToLower())).ToList();
                }
                var baranggayList = GetBaranggayList(places.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return places.Select(a => ToDto(a, baranggayList)).ToList();
            }
        }
        public static IEnumerable<PlaceDto> GetListHistory()
        {
            var homeSlug = "history-of-";
            var list = PlaceRepo.GetListByType((int)CategoryTypeEnum.Pages);
            list = list.Where(s => (s.HomeSlug ?? "").ToLower().Contains(homeSlug.ToLower()));
            return list;
        }

        public static IEnumerable<PlaceDto> GetList(IEnumerable<int> placeIds)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.Places.Include(s => s.Baranggay).Include(s => s.Baranggay.City).Where(a => !a.IsDeleted && placeIds.Contains(a.Id)).ToList();
                var baranggayList = GetBaranggayList(list.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return list.Select(a => ToDto(a, baranggayList)).ToList();
            }
        }

        public static IEnumerable<CityDto> GetListMunicipalities()
        {
            return CityRepo.GetList();
            //using (LakbayDBEntities context = new LakbayDBEntities())
            //{
            //    var type = (int)Enums.CategoryTypeEnum.Municipality;
            //    //var list = context.Places.Where(a => !a.IsDeleted && a.Type == type).Select(a => a.ParentId).ToList().Distinct();

            //    var mun = context.Places.Where(a => !a.IsDeleted && a.Type == type).ToList();

            //    return mun.Select(a => ToDto(a)).ToList();
            //}
        }

        public static IEnumerable<PlaceDto> GetListByType(int type, int parent = 0, int limit = 0, string param = "")
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                IQueryable<Place> list;


                list = context.Places.Where(a => !a.IsDeleted).OrderByDescending(h => h.DisplayPriority);


                if (type > 0)
                    list = list.Where(h => h.Type == type);

                if (parent > 0)
                    list = list.Where(h => h.ParentId == parent);

                if (!string.IsNullOrEmpty(param))
                    list = list.Where(h => h.Title.ToLower().Contains(param.ToLower()));

                if (limit > 0 && type == 0)
                {
                    try
                    {
                        var types = list.Select(s => s.Type).Distinct();
                        var newList = new List<Place>();
                        foreach (var _type in types)
                        {
                            newList.AddRange(list.Where(s => s.Type == _type).Take(limit));
                        }
                        list = newList.AsQueryable();
                    }
                    catch
                    {
                        list = list.Take(limit);
                    }
                }
                else if (limit > 0)
                {
                    list = list.Take(limit);
                }

                var baranggayList = GetBaranggayList(list.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return list.ToList().OrderByDescending(h => h.DisplayPriority).ThenBy(h => h.Title).Select(a => ToDto(a, baranggayList)).ToList();
            }
        }


        private static List<Baranggay> GetBaranggayList(int id, LakbayDBEntities context)
        {
            return context.Baranggays.Include(s => s.City).Include(s => s.City.Province).Where(s => s.Id == id).ToList();
        }

        private static List<Baranggay>GetBaranggayList(IEnumerable<int> ids, LakbayDBEntities context)
        {
            return context.Baranggays.Include(s => s.City).Include(s => s.City.Province).Where(s => ids.Contains(s.Id)).ToList();
        }

        public static IEnumerable<PlaceDto> GetList(IEnumerable<int> types, int municipalid)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                IQueryable<Place> list;

                list = context.Places.Where(a => !a.IsDeleted).OrderByDescending(h => h.DisplayPriority);

                if (municipalid > 0)
                    list = list.Where(h => h.ParentId == municipalid);

                if (types.Any())
                    list = list.Where(a => types.Contains(a.Type));
                var baranggayList = GetBaranggayList(list.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return list.ToList().Select(a => ToDto(a, baranggayList)).ToList().OrderBy(h => h.Title);
            }
        }

        public static IEnumerable<int> GetAvailableCategoriesForMunicipalPlace(int municipalid)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                IQueryable<Place> list;

                list = context.Places.Where(a => !a.IsDeleted).OrderByDescending(h => h.DisplayPriority);

                if (municipalid > 0)
                    list = list.Where(h => h.ParentId == municipalid);

                return list.Select(s => s.Type).Distinct().ToList();
            }
        }
        public static IEnumerable<int> GetAvaliableMunicipalPlacesFromSelectedCategories(List<int> selectedCategories)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                IQueryable<Place> list;

                var municipalities = PlaceRepo.GetListMunicipalities().ToList();

                list = context.Places.Where(a => !a.IsDeleted && selectedCategories.Contains(a.Type));

                var parentIds = list.Select(s => s.ParentId).Distinct().ToList();
                return municipalities.Where(s => parentIds.Contains(s.Id)).Select(s => s.Id).ToList();
            }
        }
        public static IEnumerable<PlaceDto> GetListEventsByYear(int year)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                IQueryable<Place> list;
                int type = (int)Enums.CategoryTypeEnum.NewsEvents;

                list = context.Places.Where(a => !a.IsDeleted && a.Type == type);

                if (year > 0)
                {
                    var dateFrom = new DateTime(year, 1, 1);
                    var dateTo = new DateTime(year, 12, 31, 23, 59, 59);
                    list = list.Where(h => h.PublishedDate >= dateFrom && h.PublishedDate <= dateTo);
                }
                var baranggayList = GetBaranggayList(list.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return list.ToList().Select(a => ToDto(a, baranggayList)).ToList().OrderBy(h => h.Title);
            }
        }
        public static IEnumerable<PlaceDto> GetListEvents()
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                IQueryable<Place> list;
                int type = (int)Enums.CategoryTypeEnum.NewsEvents;

                list = context.Places.Where(a => !a.IsDeleted && a.Type == type);
             
                var baranggayList = GetBaranggayList(list.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return list.ToList().Select(a => ToDto(a, baranggayList)).ToList().OrderBy(h => h.Title);
            }
        }

        public static IEnumerable<PlaceDto> GetListFavorites(int userId)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var favs = context.Favorites.Where(a => a.UserId == userId).Select(a => a.PlaceId).Distinct().ToList();
                var list = context.Places.Where(a => favs.Any(b => b == a.Id) && !a.IsDeleted).ToList();
                var baranggayList = GetBaranggayList(list.Where(s => s.BaranggayId != null).Select(s => (int)s.BaranggayId).Distinct(), context);
                return list.Select(a => ToDto(a, baranggayList)).ToList().OrderBy(h => h.Title);
            }
        }

        private static Tuple<int, int, int> GetCountryProvinceCityIds(List<Baranggay> baranggays, int baranggayId)
        {
            if (baranggays == null) return new Tuple<int, int, int>(0, 0, 0);
            var baranggay = baranggays.FirstOrDefault(s => s.Id == baranggayId);
            if (baranggay == null) return new Tuple<int, int, int>(0, 0, 0);

            var city = baranggay.City;
            var cityId = baranggay.CityId;

            var province = city?.Province;
            var provinceId = city?.ProvinceId ?? 0;

            var countryId = province?.CountryId ?? 0;

            //country province city
            return new Tuple<int, int, int>(countryId, provinceId, cityId);
        }
       

        public static PlaceDto ToDto(Place a, List<Baranggay> baranggays = null)
        {
            if (a == null) return null;

            var ids = GetCountryProvinceCityIds(baranggays, a.BaranggayId ?? 0);
            var countryId = ids.Item1;
            var provinceId = ids.Item2;
            var cityId = ids.Item3;
            return new PlaceDto
            {
                Id = a.Id,
                BaranggayId = a.BaranggayId ?? 0,
                CountryId = countryId,
                ProvinceId = provinceId,
                CityId = cityId,
                CompleteAddress = ((a.CompleteAddress) ?? "").ToUpper(),
                Title = a.Title.FixCase(),
                ContactNumber = a.ContactNumber.FixMobile(),
                Email = a.Email,
                Description = a.Description,
                DescriptionHTML = a.DescriptionHTML,
                Directions = a.Directions,
                Url = a.Url,
                ShopUrl = a.ShopURL,
                PaymentOption = (PaymentOptionEnums)a.PaymentOption,
                Type = (CategoryTypeEnum)a.Type,
                IsDeleted = a.IsDeleted,
                HomeSlug = a.HomeSlug,
                DistanceFromManila = a.DistanceFromManila ?? 0,
                TotalPlaces = a.TotalPlaces ?? 0,
                DisplayPriority = a.DisplayPriority ?? 1,
                PublishedDate = a.PublishedDate ?? DateTime.Now,
                PublishedBy = a.PublishedBy ?? "-",
                HomeThumbnail = a.HomeThumbnail ?? "default.jpg",
                ContactPerson = (a.ContactPerson ?? "-").FixCase(),
                ParentId = a.ParentId ?? 0,
                ParentName = a.City?.Name ?? "",
                HeaderVideoURL = a.HeaderVideoURL,
                MapsLatitude = a.MapsLatitude,
                MapsLongtitude = a.MapsLongtitude,
                GUID = a.GUID,
                Tagline = a.TagLine
            };
        }

        public ReturnValue Add(PlaceDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                // ToDo: create add to db function

                return new ReturnValue();
            }
        }

        public ReturnValue Delete(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                // ToDo: create delete to db function

                return new ReturnValue();
            }
        }

        public ReturnValue Update(PlaceDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                // ToDo: create update db function

                return new ReturnValue();
            }
        }
    }
}