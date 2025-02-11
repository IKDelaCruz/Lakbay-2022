using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models.NotificationModels
{
    public class NotificationDto : IFirebaseData
    {
        public NotificationDto()
        {
       
        }
      

        public string message { get; set; }
        public string title { get; set; }
        public string channel_id { get; set; }
        public int id { get; set; }
    }
}
