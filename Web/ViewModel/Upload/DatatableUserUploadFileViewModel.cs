using Project.Models.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModel.Upload
{
    public class DatatableUserUploadFileViewModel
    {
        public DatatableUserUploadFileViewModel()
        {
            t = new UserUploadFileDto();
        }
        public UserUploadFileDto t { get; set; }

        public string dateUploaded => t.DateUploaded.ToLongDateString();
        public string type => t.Type.ToString().Replace("_", " ");
        public string imgUrl { get; set; }
        public bool isFromIos { get; set; }
        public string remarks => t.Remarks ?? "";
    }
}