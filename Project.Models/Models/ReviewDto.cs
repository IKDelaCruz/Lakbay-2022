using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string Username { get; set; }
        public int ReviewStars { get; set; }
        public string ReviewText { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UserId { get; set; }

    }
}
