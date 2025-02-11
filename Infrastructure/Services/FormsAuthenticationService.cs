using Newtonsoft.Json;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Infrastructure.Services
{
    public class FormsAuthenticationService
    {
        public static void CreateCookie(AuthenticatedUserModel user, bool isPersistentLogin)
        {
            var userData = JsonConvert.SerializeObject(user);
            var encryptedData = Fletcher.Encrypt(userData);

            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: user.Username,
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
            context.Response.Cookies.Add(myCookie);
            context.Response.Cookies.Add(formsCookie);
        }

        public static AuthenticatedUserModel Identity
        {
            get
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                    return null;

                var identity = (FormsIdentity)HttpContext.Current.User.Identity;

                if (!identity.IsAuthenticated)
                    return null;

                var ticket = identity.Ticket;

                var decryptedData = Fletcher.Decrypt(ticket.UserData);

                var userInfo = JsonConvert.DeserializeObject<AuthenticatedUserModel>(decryptedData);

                return userInfo;
            }
        }

        public static void Signout()
        {
            HttpContext.Current.Session.Clear();
            FormsAuthentication.SignOut();
        }
    }
}
