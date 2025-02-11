
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Project.Repository.Repo.Sender
{
    public class SenderRepo
    {
        public static void AddQueueSms(string message, string number)
        {
            using (var context = new Database.DbContext.LakbayQueueEntities())
            {
                var rec = context.sms_queue.Create();
                rec.created_date = DateTime.Now;
                rec.recipient = number;
                rec.message = message;
                rec.sender = "app";
                rec.status = (int)QueueStatus.PENDING;
                context.sms_queue.Add(rec);
                context.SaveChanges();
            }
        }
        public static void AddQueueEmail(string message, string email, string subject = "test")
        {
            using (var context = new Database.DbContext.LakbayQueueEntities())
            {
                var rec = context.email_queue.Create();
                rec.created_date = DateTime.Now;
                rec.recipient = email;
                rec.message = message;
                rec.subject = subject;
                rec.sender = "app";
                rec.status = (int)QueueStatus.PENDING;
                context.email_queue.Add(rec);
                context.SaveChanges();
            }
        }

        public static void SendCodeEmail(string code, string email, string subject, EmailType emailType)
        {
            using (var context = new Database.DbContext.LakbayQueueEntities())
            {
                var rec = context.email_queue.Create();
                rec.created_date = DateTime.Now;
                rec.recipient = email;
                rec.message = code;
                rec.subject = subject;
                rec.sender = "app";
                rec.status = (int)QueueStatus.COMPLETE;
                context.email_queue.Add(rec);
                context.SaveChanges();
            }

            {
                Task.Run(() => EmailSender.SendCode(subject, email, code, emailType));
            }
        }

    
    }
    public enum QueueStatus
    {
        PENDING = 1,
        COMPLETE,
        FAIL
    }
}
