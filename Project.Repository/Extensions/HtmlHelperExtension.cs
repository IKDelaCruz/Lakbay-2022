using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Project.Repository.Extensions
{
    public static class HtmlHelperExtension
    {
        public static string IsActive(this HtmlHelper html, string area, string cssClass = "")
        {
            if (string.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentArea = (string)html.ViewContext.RouteData.DataTokens["area"];
            return (area.ToLower() == (currentArea != null ? currentArea.ToLower() : "")) ? cssClass : String.Empty;
        }

        public static string IsActive(this HtmlHelper html, string area, string controller, string cssClass = "")
        {
            bool isCurrentArea = false;
            bool isCurrentController = false;

            if (string.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentArea = (string)html.ViewContext.RouteData.DataTokens["area"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            isCurrentArea = (area.ToLower() == (currentArea != null ? currentArea.ToLower() : ""));
            isCurrentController = (controller.ToLower() == (currentController != null ? currentController.ToLower() : ""));

            return (isCurrentArea && isCurrentController) ? cssClass : String.Empty;
        }

        public static string IsActive(this HtmlHelper html, string area, string controller, string action, string cssClass = "")
        {
            bool isCurrentArea = false;
            bool isCurrentController = false;
            bool isCurrentAction = false;

            if (string.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentArea = (string)html.ViewContext.RouteData.DataTokens["area"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];

            isCurrentArea = (area.ToLower() == (currentArea != null ? currentArea.ToLower() : ""));
            isCurrentController = (controller.ToLower() == (currentController != null ? currentController.ToLower() : ""));
            isCurrentAction = (action.ToLower() == (currentAction != null ? currentAction.ToLower() : ""));

            return (isCurrentArea && isCurrentController && isCurrentAction) ? cssClass : String.Empty;
        }
    }
}
