using Project.Models.Enums_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models.API
{
    public class UserRegistrationDto
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        //public string Username { get; set; }
        public string Password { get; set; }
        public string SmsCode { get; set; }
        public UserLinkedAccountTypeEnums RegistrationType { get; set; }
        public string LinkId { get; set; }
        public int? Age { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public Gender? Gender { get; set; }
    }
}
