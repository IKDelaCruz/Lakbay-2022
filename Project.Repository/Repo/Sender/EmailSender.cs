using Project.Repository.Repo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
namespace Project.Repository.Repo.Sender
{
    public enum EmailType
    {
        VerificationCode,
        ForgotPassword,
    }
    public class EmailSender
    {

        public static string USERNAME = "hello@travelorientalmindoro.ph";
        public static string PASSWORD = "svdphvfwfrrosryt";


        private static AlternateView PlaceEmailHeaderLogo(ref string body, string imagePath)
        {
            LinkedResource res = new LinkedResource(imagePath);
            res.ContentId = Guid.NewGuid().ToString();

            body = body.Replace("@!-image-!@", res.ContentId);

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        private static string GetLoginUrl()
        {
            return ConfigurationManager.AppSettings["loginUrl"];
        }

        public static void SendCode(string subject, string receiver, string code, EmailType emailType)
        {
            try
            {
                var destinations = new List<string>();

                var date = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

                MailMessage message = new MailMessage();
                message.From = new MailAddress(USERNAME);
                message.To.Add(receiver.ToString());
                message.Subject = subject;
                message.IsBodyHtml = true;
                var loginUrl = GetLoginUrl();
                switch (emailType)
                {
                    case EmailType.VerificationCode:
                        message.Body = BuildVerificationCodeTemplate(code);
                        break;
                    case EmailType.ForgotPassword:
                        message.Body = BuildForgotPasswordTemplate(code, loginUrl);
                        break;
                }
                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential()
                    {
                        UserName = USERNAME,
                        Password = PASSWORD,
                    };
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                LogsRepo.Add(null, ex.Message);
            }

          
        }

        public static string BuildVerificationCodeTemplate(string code)
        {
            var template = string.Empty;
            template += "<html>";
            template += "   <head></head>";
            template += "   <body>";
            template += "       <div style='padding: 25px; max-width: 500px;'>";
            template += "           <img src='https://travelorientalmindoro.ph/content/img/logo-transparent.png' style='height: 100px; display:block; margin-left:auto; margin-right:auto; width:50%;'  class=''/>";
            template += "           <p style='font-size: 25px; font-family: arial; margin-bottom: -5px; padding-left: 25px;'>Verify your email address</p>";
            template += "           <p style='font-size: 15px; font-family: arial; margin-bottom: 25px; padding-left: 25px;'>To verify your email address, enter this code.</p>";
            template += "           <div style='text-align: center; background-color: lightgrey; margin-left: 25px; padding: 50px; width: auto;'>";
            template += $"              <label style='font-size: 50px; font-family: arial; margin-bottom: 0;'>{code}</label>";
            template += "           </div>";
            template += "           <p style='font-size: 15px; font-family: arial; margin-bottom: 25px; padding-left: 25px;'>";
            template += "               If you didn't request a code, you can safely ignore this email.";
            template += "           </p>";
            template += "       </div>";
            template += "   </body>";
            template += "</html>";

            return template;
        }

        public static string BuildForgotPasswordTemplate(string code, string loginUrl)
        {
            var template = string.Empty;
            template += "<html>";
            template += "   <head></head>";
            template += "   <body>";
            template += "       <div style='padding: 25px; max-width: 500px;'>";
            template += "           <img src='https://travelorientalmindoro.ph/content/img/logo-transparent.png' style='height: 100px; display:block; margin-left:auto; margin-right:auto; width:50%;' class=''/>";
            template += "           <p style='font-size: 25px; font-family: arial; margin-bottom: -5px;  text-align: center;'>Temporary Password</p>";
            template += "           <p style='font-size: 15px; font-family: arial; margin-bottom: -5px; padding-left: 25px;'>You can find below your temporary password.</p>";
            //template += "           <p style='font-size: 15px; font-family: arial; margin-bottom: 25px; padding-left: 25px;'>To complete your registration, enter this code in the dList app.</p>";
            template += "           <div style='text-align: center; background-color: lightgrey; margin-left: 25px; padding: 50px; width: auto;'>";
            template += $"              <label style='font-size: 50px; font-family: arial; margin-bottom: 0;'>{code}</label>";
            template += "           </div>";
            //template += "           <p style='font-size: 15px; font-family: arial; margin-bottom: 25px; padding-left: 25px;'>";
            //template += "               If you didn't request a code, you can safely ignore this email.";
            //template += "           </p>";
            template += "            <div style='text-align:center; margin-top:10px;'>";
            template +=$"               <a href='{loginUrl}' style='  background-color: #136319; border: none; " +
                                        "color: white;padding: 15px 32px; text-align: center;text-decoration: none;display: " +
                                        "inline-block;font-size: 16px; font-family:tahoma;'>" +
                                        "Login to your account" +
                                        "</a>";
            template += "            </div>";
            template += "       </div>";
            template += "   </body>";
            template += "</html>";

            return template;
        }

   
    }
}
