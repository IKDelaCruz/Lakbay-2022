using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models.UserModels
{
    public class UserPlaceVisitHistoryDto
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public int PlaceId { get; set; }
        public DateTime DateVisit { get; set; }
        public string Remarks { get; set; }
    }
}
