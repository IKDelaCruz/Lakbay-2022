using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.API.Authentication
{
    public class ChangePasswordModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public bool AuthenticateOldPassword { get; set; }
    }
}
