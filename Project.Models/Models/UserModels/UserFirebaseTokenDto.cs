using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models
{
    public class UserFirebaseTokenDto
    {
        public int UserId { get; set; }
        public string FirebaseToken { get; set; }
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
