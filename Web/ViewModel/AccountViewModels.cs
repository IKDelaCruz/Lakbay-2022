using Foolproof;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web.ViewModel;
using static Project.Models.Enums;

namespace Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string LinkId { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel : BaseViewModel
    {
       // [Required]
        public string Provider { get; set; }

        public UserLinkedAccountTypeEnums Type { get; set; }
        public string LinkId { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string _code { get; set; }

        public string ReturnUrl { get; set; }
        public int LinkAccountId { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage ="Please enter a mobile number or email address")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public UserLinkedAccountTypeEnums Type
        {
            get
            {
                return IsValidEmail(Username) ? UserLinkedAccountTypeEnums.Email : UserLinkedAccountTypeEnums.Mobile;
            }
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public class RegisterViewModel : BaseViewModel
    {
        public RegisterViewModel()
        {
            Type = UserLinkedAccountTypeEnums.Mobile;
        }

        //  [RequiredIfTrue("IsEmail", ErrorMessage = "Email is required")]
        [RequiredIf("Type", UserLinkedAccountTypeEnums.Email, ErrorMessage = "Email is required")]
        //[Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //  [RequiredIfTrue("IsMobile", ErrorMessage = "Mobile number is required")]
        // [Required]
        [RequiredIf("Type", UserLinkedAccountTypeEnums.Mobile, ErrorMessage = "Mobile number is required")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile number")]
        [RegularExpression(@"^(\d{11})$", ErrorMessage = "Invalid mobile number")]
        public string MobileNumber { get; set; }

        //public bool RequirePassword
        //{
        //    get
        //    {
        //        if(Type == UserLinkedAccountTypeEnums.Mobile && !string.IsNullOrEmpty(MobileNumber))
        //        {
        //            return true;
        //        }
        //        else if (Type == UserLinkedAccountTypeEnums.Email && !string.IsNullOrEmpty(Email))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

       // [RequiredIfTrue("RequirePassword", ErrorMessage = "Password is required")]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string GetUsername()
        {
            if (Type == UserLinkedAccountTypeEnums.Email)
                return Email;
            else if (Type == UserLinkedAccountTypeEnums.Mobile)
                return MobileNumber;
            return "";
        }


        public UserLinkedAccountTypeEnums Type { get; set; }
        [NotMapped]
        public bool IsEmail => Type == UserLinkedAccountTypeEnums.Email;
        [NotMapped]
        public bool IsMobile => Type == UserLinkedAccountTypeEnums.Mobile;

        public UserLinkedAccountTypeEnums EmailType => UserLinkedAccountTypeEnums.Email;
        public UserLinkedAccountTypeEnums MobileType => UserLinkedAccountTypeEnums.Mobile;

        public string hiddenCode { get; set; }
        public string code { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel : BaseViewModel
    {
        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        [Required(ErrorMessage ="Email address or mobile number is required.")]
        public string Username { get; set; }
    }
}
