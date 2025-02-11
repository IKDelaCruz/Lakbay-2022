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
    public class ProvinceRepo
    {
        public static IEnumerable<ProvinceDto> GetList(int countryId = 0)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.Provinces.Where(a => !a.IsDeleted);

                if (countryId > 0)
                    list = list.Where(a => a.CountryId == countryId);

                return list.Include(a => a.Country).Select(a => new ProvinceDto()
                {
                    Id = a.Id,
                    CountryId = a.Country != null ? a.Country.Id : 0,
                    CountryName = a.Country != null ? a.Country.Name : "",
                    Name = a.Name,
                    IsDeleted = a.IsDeleted
                }).ToList();
            }
        }

        public static ProvinceDto Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = context.Provinces.Where(a => a.Id == id).Include(a => a.Country).FirstOrDefault();

                if (item == null)
                    return null;

                return new ProvinceDto()
                {
                    Id = item.Id,
                    CountryId = item.Country?.Id ?? 0,
                    CountryName = item.Country?.Name ?? "",
                    Name = item.Name,
                    IsDeleted = item.IsDeleted
                };
            }
        }

        public ReturnValue Add(ProvinceDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Provinces.Where(a => (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("Province [" + model.Name + "] is already exists in the system.");

                var item = new Province()
                {
                    Id = model.Id,
                    CountryId = model.CountryId,
                    Name = model.Name,
                    IsDeleted = model.IsDeleted
                };

                context.Provinces.Add(item);
                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Province [" + model.Name + "] has been successfully added to the system.", true, item.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to add province [" + model.Name + "] to the system."));

                return result;
            }
        }

        public ReturnValue Update(ProvinceDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Provinces.Where(a => (a.Id != model.Id) && (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("Province [" + model.Name + "] is already exists in the system.");

                var record = context.Provinces.FirstOrDefault(a => a.Id == model.Id);

                if (record == null)
                    return new ReturnValue("Unable to find province [" + model.Name + "].");

                var oldName = record.Name;

                record.CountryId = model.CountryId;
                record.Name = model.Name;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Province [" + model.Name + "] has been successfully updated.", true, model.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to update province [" + oldName + "] to the system."));

                return result;
            }
        }

        public ReturnValue Delete(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.Provinces.FirstOrDefault(a => a.Id == id);

                if (record == null)
                    return new ReturnValue("Unable to find province.");

                record.IsDeleted = true;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Province [" + record.Name + "] has been successfully updated.", true) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to delete province [" + record.Name + "] to the system."));

                return result;
            }
        }

    }
}
