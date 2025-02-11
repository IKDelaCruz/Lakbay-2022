using Project.Database.DbContext;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repository.Repo
{
    public class CountryRepo
    {
        public static IEnumerable<CountryDto> GetList()
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var list = context.Countries.Where(a => !a.IsDeleted);

                return list.Select(a => new CountryDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsDeleted = a.IsDeleted
                }).ToList();
            }
        }

        public static CountryDto Get(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var item = context.Countries.FirstOrDefault(a => a.Id == id);

                if (item == null)
                    return null;

                return new CountryDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsDeleted = item.IsDeleted
                };
            }
        }

        public ReturnValue Add(CountryDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Countries.Where(a => (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("Country [" + model.Name + "] is already exists in the system.");

                var item = new Country()
                {
                    Id = model.Id,
                    Name = model.Name,
                    IsDeleted = model.IsDeleted
                };

                context.Countries.Add(item);
                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Country [" + model.Name + "] has been successfully added to the system.", true, item.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to add country [" + model.Name + "] to the system."));

                return result;
            }
        }

        public ReturnValue Update(CountryDto model)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var records = context.Countries.Where(a => (a.Id != model.Id) && (a.Name.ToLower() == model.Name.ToLower()) && !a.IsDeleted);

                if (records.Any())
                    return new ReturnValue("Country [" + model.Name + "] is already exists in the system.");

                var record = context.Countries.FirstOrDefault(a => a.Id == model.Id);

                if (record == null)
                    return new ReturnValue("Unable to find country [" + model.Name + "].");

                var oldName = record.Name;
                record.Name = model.Name;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Country [" + model.Name + "] has been successfully updated.", true, model.Id) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to update country [" + oldName + "] to the system."));

                return result;
            }
        }

        public ReturnValue Delete(int id)
        {
            using (LakbayDBEntities context = new LakbayDBEntities())
            {
                var record = context.Countries.FirstOrDefault(a => a.Id == id);

                if (record == null)
                    return new ReturnValue("Unable to find country.");

                record.IsDeleted = true;

                var dbSaveChanges = context.SaveChanges();
                ReturnValue result = dbSaveChanges > 0 ?
                                new ReturnValue("Country [" + record.Name + "] has been successfully updated.", true) :
                            (dbSaveChanges == 0 ?
                                new ReturnValue("Nothing to save.") :
                                new ReturnValue("Unable to delete country [" + record.Name + "] to the system."));

                return result;
            }
        }
        
    }
}
