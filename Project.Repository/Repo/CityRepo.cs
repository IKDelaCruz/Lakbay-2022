using Project.Database.DbContext;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repository.Repo
{
    public class CityRepo
    {
        public static CityDto Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = context.Cities.Where(a => a.Id == id).Include(a => a.Province).FirstOrDefault();

                if (item == null)
                    return null;

                return new CityDto()
                {
                    Id = item.Id,
                    CountryId = item.Province?.CountryId ?? 0,
                    ProvinceId = item.Province?.Id ?? 0,
                    ProvinceName = item.Province?.Name ?? "",
                    Name = item.Name,
                    IsDeleted = item.IsDeleted
                };
            }
        }

        public static CityDto GetByName(string name)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = context.Cities.Where(a => a.Name == name).Include(a => a.Province).FirstOrDefault();

                if (item == null)
                    return null;

                return new CityDto()
                {
                    Id = item.Id,
                    CountryId = item.Province?.CountryId ?? 0,
                    ProvinceId = item.Province?.Id ?? 0,
                    ProvinceName = item.Province?.Name ?? "",
                    Name = item.Name,
                    IsDeleted = item.IsDeleted
                };
            }
        }

        public static CityDto GetByPlaceId(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = context.Cities.FirstOrDefault(a => a.PlaceId == id);

                if (item == null)
                    return null;

                return new CityDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsDeleted = item.IsDeleted
                };
            }
        }

        public static IEnumerable<CityDto> GetList(int provinceId = 0)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.Cities.Where(a => !a.IsDeleted);

                if (provinceId > 0)
                    list = list.Where(a => a.ProvinceId == provinceId);

                return list.Include(a => a.Province).Select(a => new CityDto()
                {
                    Id = a.Id,
                    CountryId = a.Province != null ? a.Province.CountryId : 0,
                    ProvinceId = a.Province != null ? a.Province.Id : 0,
                    ProvinceName = a.Province != null ? a.Province.Name : "",
                    Name = a.Name,
                    IsDeleted = a.IsDeleted
                }).ToList();
            }
        }

        public ReturnValue Add(CityDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Cities.Where(a => (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("City [" + model.Name + "] is already exists in the system.");

                var item = new City()
                {
                    Id = model.Id,
                    ProvinceId = model.ProvinceId,
                    Name = model.Name,
                    IsDeleted = model.IsDeleted
                };

                context.Cities.Add(item);
                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("City [" + model.Name + "] has been successfully added to the system.", true, item.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to add city [" + model.Name + "] to the system."));

                return result;
            }
        }

        public ReturnValue Delete(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.Cities.FirstOrDefault(a => a.Id == id);

                if (record == null)
                    return new ReturnValue("Unable to find city.");

                record.IsDeleted = true;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("City [" + record.Name + "] has been successfully updated.", true) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to delete city [" + record.Name + "] to the system."));

                return result;
            }
        }

        public ReturnValue Update(CityDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Cities.Where(a => (a.Id != model.Id) && (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("City [" + model.Name + "] is already exists in the system.");

                var record = context.Cities.FirstOrDefault(a => a.Id == model.Id);

                if (record == null)
                    return new ReturnValue("Unable to find city [" + model.Name + "].");

                var oldName = record.Name;

                record.ProvinceId = model.ProvinceId;
                record.Name = model.Name;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("City [" + model.Name + "] has been successfully updated.", true, model.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to update city [" + oldName + "] to the system."));

                return result;
            }
        }
    }
}