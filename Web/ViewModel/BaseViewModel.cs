using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModel
{
    public class BaseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string FBTitle { get; set; }
        public string FBDescription { get; set; }
        public string FBImageURL { get; set; }
        public string FBContentURL { get; set; }
        public BaseViewModel()
        {
            FBTitle = "LAKBAY | Travel Oriental Mindoro";
            FBDescription = "Oriental Mindoro is an image of tropical paradise - white sand beaches, clear blue waters, coral reefs in varied colors, cool rushing rivers, hidden waterfalls and lush green forests covering majestic mountains. With its largely unspoiled natural beauty, the province has much to offer in terms of tourism and investment.";
            FBImageURL = "https://travelorientalmindoro.ph/Content/img/uploads/default.jpg";
            FBContentURL = "https://travelorientalmindoro.ph/";

        }
    }
}