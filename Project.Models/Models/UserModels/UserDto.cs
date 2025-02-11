using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project.Models.Enums;

namespace Project.Models.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordKey { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastLoginAttempt { get; set; }
        public int LoginCounter { get; set; }
        public bool IsLocked { get; set; }
        public DateTime DateDeleted { get; set; }
        public bool IsDeleted { get; set; }
        public string GUID { get; set; }
        public string TouristId { get; set; }
  
    }

    public class UserLinkedAccountsModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LinkId { get; set; }
        public UserLinkedAccountTypeEnums Type { get; set; }
        public DateTime DateLinked { get; set; }
    }
}
