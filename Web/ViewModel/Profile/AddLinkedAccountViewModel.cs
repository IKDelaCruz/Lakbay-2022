using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Project.Models.Enums;

namespace Web.ViewModel.Profile
{
    public class AddLinkedAccountViewModel
    {
        public UserLinkedAccountTypeEnums Type { get; set; }
        public string GetUsername() 
        { 
            if (Type == UserLinkedAccountTypeEnums.Email)
                return Email;
            else if (Type == UserLinkedAccountTypeEnums.Mobile)
                return MobileNumber;
            return "";
        }

        [Required(ErrorMessage = "Please enter an email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a mobile number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\d{11})$", ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }

    }
}