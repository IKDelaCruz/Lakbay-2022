using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models.NotificationModels
{
    public interface IFirebaseData
    {
        int id { get; set; }
        string message { get; set; }
        string title { get; set; }
        string channel_id { get; set; }
    }
}
