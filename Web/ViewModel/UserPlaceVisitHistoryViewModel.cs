using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModel
{
    public class UserPlaceVisitHistoryViewModel : BaseViewModel
    {
        public UserPlaceVisitHistoryViewModel()
        {
            CDNHOST = System.Configuration.ConfigurationManager.AppSettings["CDNHOST"];
        }
        public string CDNHOST { get; set; }
        public int UserId { get; set; }
        public List<UserPlaceVisitModel> List { get; set; }
    }
}