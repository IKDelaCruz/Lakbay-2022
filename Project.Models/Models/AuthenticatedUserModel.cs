using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models
{
    public class AuthenticatedUserModel : UserDetailDto
    {
        public string Username { get; set; }
        public UserRole Role { get; set; }

        public bool HasRole(UserRole role)
        {
            return (role == Role);
        }
    }
}
