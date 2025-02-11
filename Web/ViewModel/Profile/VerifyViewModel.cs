using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.ViewModel.Profile
{
    public class VerifyViewModel    
    {
        public AddLinkedAccountViewModel LinkedAccount { get; set; }

        [Required(ErrorMessage = "Enter verification code")]
        public string VerifyCode { get; set; }
        public string hiddenVerifyCode { get; set; }
    }
}