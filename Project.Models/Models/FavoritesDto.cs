using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models
{
    public class FavoritesDto
    {
        public FavoritesDto()
        {
            Place = new PlaceDto();
        }
        public int Id { get; set; }
        public PlaceDto Place { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
    }
}
