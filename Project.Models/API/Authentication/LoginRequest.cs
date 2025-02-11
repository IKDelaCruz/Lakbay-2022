using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models.API.Authentication
{
    public class LoginRequest
    {
        //for userlink- display email or mobile
        public string Description { get; set; }


        public string Mobile { get; set; }
        public string Email { get; set; }


        public string Password { get; set; }
        public UserLinkedAccountTypeEnums Type { get; set; }
        public string LinkId { get; set; }

        //for registration
        public string Name { get; set; }
        public string ProfilePicURL { get; set; }
    }
}
