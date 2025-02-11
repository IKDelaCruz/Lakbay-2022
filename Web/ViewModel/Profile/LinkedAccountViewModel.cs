using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Project.Models.Enums;

namespace Web.ViewModel.Profile
{
    public class LinkedAccountViewModel
    {
        public LinkedAccountViewModel()
        {
            dto = new UserLinkedAccountDto();
        }
        public UserLinkedAccountDto dto { get; set; }
        public string Description => dto?.Description ?? "";
        public string LinkClass => dto?.Id == 0 ? "" : "bg-danger";
        public string LinkText => dto?.Id == 0 ? "Link" : "Unlink";
        public UserLinkedAccountTypeEnums Type { get; set; }
        public int Id => dto?.Id ?? 0;
       
    }
}