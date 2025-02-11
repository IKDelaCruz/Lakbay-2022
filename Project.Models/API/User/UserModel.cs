using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.API.User
{
    public class UserModel
    {
        public UserDto User { get; set; }
        public UserDetailDto Details { get; set; }
    }
}
