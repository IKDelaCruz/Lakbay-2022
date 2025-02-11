using Project.Models.Enums_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Models
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool IsDeleted { get; set; }

        public Gender? Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int? Age { get; set; }

        public string ProfilePictureUrl { get; set; }
    }
}
