using Newtonsoft.Json;
using Project.Models;
using Project.Models.API.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Infrastructure.Services.WebAuthentication
{
    public class WebFormsAuthenticationService
    {
        public static void CreateCookie(AuthUserModel user, bool isPersistentLogin)
        {
            var userData = JsonConvert.SerializeObject(user);
            var encryptedData = Fletcher.Encrypt(userData);

            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: user.Guid,
                issueDate: DateTime.UtcNow,
                expiration: isPersistentLogin ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(30),
                isPersistent: isPersistentLogin,
                userData: encryptedData
                );

            var userPrincipal = new GenericPrincipal(new GenericIdentity(ticket.Name), null);

            HttpContext.Current.User = userPrincipal;

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var context = HttpContext.Current;

            var formsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

          

            HttpCookie myCookie = new HttpCookie("TravelOrientalMindoro");
            myCookie.Values.Add("XID", user.Id.ToString());
            myCookie.Expires = DateTime.Now.AddHours(36);

            HttpCookie myCookie2 = new HttpCookie("LakbayTravelOrientalMindoro");
            myCookie2.Values.Add("user", encryptedData);
            myCookie2.Expires = DateTime.Now.AddHours(36);
            myCookie2.Domain = ".travelorientalmindoro.ph";



            context.Response.Cookies.Add(myCookie2);
            context.Response.Cookies.Add(myCookie);
            context.Response.Cookies.Add(formsCookie);
        }

        public static AuthUserModel Identity
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                    return null;

                var identity = HttpContext.Current.User.Identity as FormsIdentity;

                if (!(identity?.IsAuthenticated == true))
                    return null;

                var ticket = identity.Ticket;

                var decryptedData = Fletcher.Decrypt(ticket.UserData);

                var userInfo = JsonConvert.DeserializeObject<AuthUserModel>(decryptedData);

                return userInfo;
            }
        }

        public static void Signout()
        {
            HttpContext.Current.Session.Clear();
            FormsAuthentication.SignOut();
            HttpCookie cookie2 = new HttpCookie("LakbayTravelOrientalMindoro")
            {
                Expires = DateTime.Now.AddDays(-7),
                Domain = ".travelorientalmindoro.ph"
            };
            HttpContext.Current.Response.Cookies.Add(cookie2);


        }
    }
}
