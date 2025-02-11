using Project.Database.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repository.Repo
{
    public class LogsRepo
    {
        public static void Add(int? userId, string message)
        {
            using(var context = new LakbayDBEntities())
            {
                context.Logs.Add(new Log
                {
                    UserId = userId,
                    Message = message,
                    Date = DateTime.Now
                });
                context.SaveChanges();
            }
        }
    }
}
