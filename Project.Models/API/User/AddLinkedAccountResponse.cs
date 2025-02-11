using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.API.User
{
    public class AddLinkedAccountResponse
    {
        public bool NeedToCreatePassword { get; set; }
        public UserLinkedAccountDto Account { get; set; }
    }
}
