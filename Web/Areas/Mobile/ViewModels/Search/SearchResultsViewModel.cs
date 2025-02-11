using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Mobile.ViewModels
{
    public class SearchResultsViewModel
    {
        public string param { get; set; }
        public int UserId { get; set; }
        public bool IsFavourite { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public bool showNearest { get; set; }
    }
}