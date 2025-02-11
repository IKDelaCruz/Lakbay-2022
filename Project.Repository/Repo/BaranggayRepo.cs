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
    public class BaranggayRepo
    {
        public static IEnumerable<BaranggayDto> GetList()
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = from a in context.Baranggays.Where(a => !a.IsDeleted)
                           select new BaranggayDto()
                           {
                               Id = a.Id,
                               CountryId = a.City != null && a.City.Province != null ? a.City.Province.CountryId : 0,
                               ProvinceId = a.City != null && a.City.Province != null ? a.City.Province.Id : 0,
                               ProvinceName = a.City != null && a.City.Province != null ? a.City.Province.Name : "",
                               CityId = a.City != null ? a.City.Id : 0,
                               CityName = a.City != null ? a.City.Name : "",
                               Name = a.Name,
                               IsDeleted = a.IsDeleted,
                               Latitude = a.Latitude,
                               Longitude = a.Longitude,
                               ZipCode = a.ZipCode
                           };

                return list.ToList();
            }
        }

        public static BaranggayDto Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = (from a in context.Baranggays.Where(a => a.Id == id)
                            select new BaranggayDto()
                            {
                                Id = a.Id,
                                CountryId = a.City != null && a.City.Province != null ? a.City.Province.CountryId : 0,
                                ProvinceId = a.City != null && a.City.Province != null ? a.City.Province.Id : 0,
                                ProvinceName = a.City != null && a.City.Province != null ? a.City.Province.Name : "",
                                CityId = a.City != null ? a.City.Id : 0,
                                CityName = a.City != null ? a.City.Name : "",
                                Name = a.Name,
                                IsDeleted = a.IsDeleted,
                                Latitude = a.Latitude,
                                Longitude = a.Longitude,
                                ZipCode = a.ZipCode
                            }).FirstOrDefault();

                if (item == null)
                    return null;

                return item;
            }
        }

        public ReturnValue Add(BaranggayDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Baranggays.Where(a => (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("Baranggay [" + model.Name + "] is already exists in the system.");

                var item = new Baranggay()
                {
                    Id = model.Id,
                    CityId = model.CityId,
                    Name = model.Name,
                    IsDeleted = model.IsDeleted,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    ZipCode = model.ZipCode
                };

                context.Baranggays.Add(item);
                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Baranggay [" + model.Name + "] has been successfully added to the system.", true, item.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to add baranggay [" + model.Name + "] to the system."));

                return result;
            }
        }

        public ReturnValue Update(BaranggayDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Baranggays.Where(a => (a.Id != model.Id) && (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("Baranggay [" + model.Name + "] is already exists in the system.");

                var record = context.Baranggays.FirstOrDefault(a => a.Id == model.Id);

                if (record == null)
                    return new ReturnValue("Unable to find baranggay [" + model.Name + "].");

                var oldName = record.Name;

                record.CityId = model.CityId;
                record.Name = model.Name;
                record.Latitude = model.Latitude;
                record.Longitude = model.Longitude;
                record.ZipCode = model.ZipCode;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Baranggay [" + model.Name + "] has been successfully updated.", true, model.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to update baranggay [" + oldName + "] to the system."));

                return result;
            }
        }

        public ReturnValue Delete(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.Baranggays.FirstOrDefault(a => a.Id == id);

                if (record == null)
                    return new ReturnValue("Unable to find baranggay.");

                record.IsDeleted = true;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Baranggay [" + record.Name + "] has been successfully updated.", true) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to delete baranggay [" + record.Name + "] to the system."));

                return result;
            }
        }

    }
}
