using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.API.Authentication
{
    public class AuthUserModel
    {
        public AuthUserModel()
        {
            Id = 0;
            FacebookId = string.Empty;
            GmailId = string.Empty;
            AppleId = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            ProfilePicturePath = string.Empty;
            JWT_Token = string.Empty;
            Guid = string.Empty;
        }

        public int Id { get; set; }
        public string FacebookId { get; set; }
        public string GmailId { get; set; }
        public string AppleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public string ProfilePicturePath { get; set; }
        public string JWT_Token { get; set; }
        public string Guid { get; set; }
        public bool IsAdmin { get; set; }

        public string AgeDisplay { get { return Age > 0 ? Age.ToString() : ""; } }
    }
}
