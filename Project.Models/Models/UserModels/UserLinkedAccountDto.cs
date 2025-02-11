using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models.Models.UserModels
{
    public class UserLinkedAccountDto
    {
        public int Id { get; set; }
        public string LinkId { get; set; }
        public int UserId { get; set; } 
        public string Description { get; set; }
        public UserLinkedAccountTypeEnums Type { get; set; }
        public bool? IsVerified { get; set; }
    }
}
