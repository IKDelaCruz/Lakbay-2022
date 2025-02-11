using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Infrastructure.Services.WebAuthentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Project.Models;
using Project.Models.API;
using Project.Models.API.Authentication;
using Project.Models.Models;
using Project.Repository.Repo;
using Project.Repository.Repo.Users;
using Web.Models;
using static Project.Models.Enums;
using Infrastructure;

namespace Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
      

        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "/Profile")
        {
            FormsAuthentication.SignOut();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


 
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var request = new LoginRequest
            {
                LinkId = model.Username,
                Password = model.Password,
                Type = model.Type
            };

            var result = AuthenticationRepo.AuthenticateLink(request);

            if(result.Success)
            {
              
                WebFormsAuthenticationService.CreateCookie(result.ReturnData, model.RememberMe);
                //var isVerified = UserLinkedAccountsRepo.IsVerified(result.ReturnData.Id, request.Type);
                //if (isVerified == false)
                //{
                //    return RedirectToAction("VerifyCode", 
                //        new 
                //        { 
                //            type = request.Type,
                //            returnUrl = returnUrl,
                //            rememberMe = model.RememberMe 
                //        });
                //}
                //else
                {
                    return RedirectToLocal(returnUrl);
                }
              
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult SendCode(string username, UserLinkedAccountTypeEnums type)
        {
            var isExisting = UserRepo.IsLinkExisting(type, username);
            if ((bool)isExisting.ReturnParam)
            {
                var jResult = Json(new
                {
                    returnData = isExisting,
                    username = username
                }, JsonRequestBehavior.AllowGet);
                jResult.MaxJsonLength = int.MaxValue;
                return jResult;
            }
            else
            {
                var retVal = new ReturnValue();
                if (type == UserLinkedAccountTypeEnums.Email)
                    retVal = AuthenticationRepo.SendEmailVerificationCode(username);
                else
                    retVal = AuthenticationRepo.SendMobileVerificationCode(username);
                var jResult = Json(new
                {
                    returnData = retVal,
                    username = username
                }, JsonRequestBehavior.AllowGet);
                jResult.MaxJsonLength = int.MaxValue;
                return jResult;
            }
   
        }
        //
        // GET: /Account/VerifyCode
        [Authorize]
        public async Task<ActionResult> VerifyCode(UserLinkedAccountTypeEnums type, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            //if (!await SignInManager.HasBeenVerifiedAsync())
            //{
            //    return View("Error");
            //}

            var linkedActs = UserLinkedAccountsRepo.GetList(UserId);
            var account = linkedActs.FirstOrDefault(s => s.Type == type);
            var ret = new ReturnValue();

            //if(type == UserLinkedAccountTypeEnums.Email)
            //{
            //    AuthenticationRepo.SendEmailVerificationCode(account.LinkId);
            //}
            //else if(type == UserLinkedAccountTypeEnums.Mobile)
            //{
            //    AuthenticationRepo.SendMobileVerificationCode(account.LinkId);
            //}

            return View(
                new VerifyCodeViewModel
                { 
                    LinkAccountId = account.Id,
                    LinkId = account.LinkId,
                    Type = type, 
                    ReturnUrl = returnUrl, 
                    RememberMe = rememberMe,
                    _code = ret.ReturnParam as string,
                });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [Authorize]
        // [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if(model._code == model.Code)
            {
                var result = UserLinkedAccountsRepo.VerifyLinkedAccount(model.LinkAccountId);
                return RedirectToLocal(model.ReturnUrl);
            }

            ModelState.AddModelError("", "Invalid code.");
            return View(model);
            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            //var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(model.ReturnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid code.");
            //        return View(model);
            //}
        }

        [AllowAnonymous]
        public ActionResult Register2()
        {
            return View(new RegisterViewModel());
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
      //  [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            model.Success = false;
            if (ModelState.IsValid)
            {
                if (model.code == model.hiddenCode)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var registration = new UserRegistrationDto
                    {
                        LinkId = model.GetUsername(),
                        RegistrationType = model.Type,
                        Password = model.Password,
                        MobileNumber = model.MobileNumber ?? "",
                        Email = model.Email ?? ""
                    };
                    var result = UserRepo.EmailMobileRegistration(registration);


                    //var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Success)
                    {
                        WebFormsAuthenticationService.CreateCookie(result.ReturnData, false);
                        var linkedAccounts = UserLinkedAccountsRepo.GetList(result.ReturnData.Id);

                        var linked = linkedAccounts.FirstOrDefault(s => s.Type == model.Type);
                        UserLinkedAccountsRepo.VerifyLinkedAccount(linked.Id);

                        return RedirectToLocal("");


                    }
                    else
                    {
                        model.Message = result.Message;
                        return View(model);
                    }
                }
                else
                {
                    model.Message = "Invalid verification code";
                    return View(model);
                }

                
            }
            model.Message = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            throw new NotImplementedException();
            return null;
            //if (userId == null || code == null)
            //{
            //    return View("Error");
            //}
            //var result = await UserManager.ConfirmEmailAsync(userId, code);
            //return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isEmail = model.Username.IsEmail();
                if (isEmail)
                {
                    var res = UserRepo.ForgotPassword(model.Username);

                    if (res.Success)
                    {
                        TempData["message"] = "Please check your email to reset your password.";
                        return RedirectToAction("login");
                        //   return View("ForgotPasswordConfirmation");
                    }
                }
                else
                {
                    var res = UserRepo.ForgotPassword(model.Username);

                    if (res.Success)
                    {
                        TempData["message"] = "Temporary password has been sent to your mobile number";
                        return RedirectToAction("login");
                        //   return View("ForgotPasswordConfirmation");
                    }
                }
             

                //var user = await UserManager.FindByNameAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return View("ForgotPasswordConfirmation");
                //}

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            model.Message = "Invalid email.";
            model.Success = false;
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            throw new NotImplementedException();
            return null;
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            //var user = await UserManager.FindByNameAsync(model.Email);

            //if (user == null)
            //{
            //    // Don't reveal that the user does not exist
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            //if (result.Succeeded)
            //{
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //AddErrors(result);
            //return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            if(returnUrl == "/Account/ExternalLogin")
            {
                returnUrl = "/Profile";
            }
            // Request a redirect to the external login provider
            //if (string.IsNullOrEmpty(returnUrl))
            //{
            //    returnUrl = "/profile";
            //}
            //if(returnUrl == "/Account/ExternalLogin")
            //{
            //    returnUrl = "/profile";
            //}
            // https://stackoverflow.com/questions/20737578/asp-net-sessionid-owin-cookies-do-not-send-to-browser
            Session["Workaround"] = 0;
            var url = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            return new ChallengeResult(provider, url);
        }

        //
        // GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    throw new NotImplementedException();
        //    return null;
        //    //var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    //if (userId == null)
        //    //{
        //    //    return View("Error");
        //    //}
        //    //var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    //var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    //return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        //
        // POST: /Account/SendCode
     //   [HttpPost]
     //   [AllowAnonymous]
     ////   [ValidateAntiForgeryToken]
     //   public async Task<ActionResult> SendCode(SendCodeViewModel model)
     //   {
     //       throw new NotImplementedException();
     //       return null;
     //       //if (!ModelState.IsValid)
     //       //{
     //       //    return View();
     //       //}

     //       //// Generate the token and send it
     //       //if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
     //       //{
     //       //    return View("Error");
     //       //}
     //       //return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
     //   }
 
        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            if (returnUrl == null)
                returnUrl = "/";
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            var loginInfo = await authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            var type = UserLinkedAccountTypeEnums.Facebook;
            if (loginInfo.Login.LoginProvider.ToLower().Contains("facebook"))
            {
                type = UserLinkedAccountTypeEnums.Facebook;
            }
            else if(loginInfo.Login.LoginProvider.ToLower().Contains("google"))
            {
                type = UserLinkedAccountTypeEnums.Gmail;
            }

            var res = AuthenticationRepo.AuthenticateLink(new LoginRequest
            {
                LinkId = loginInfo.Login.ProviderKey,
                Type = type,
                Name = loginInfo.ExternalIdentity.Name,
                Email = loginInfo.Email,
                Description = loginInfo.Email
            });

            if (res.Success)
            {
                WebFormsAuthenticationService.CreateCookie(res.ReturnData, false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, LinkId = loginInfo.Login.ProviderKey });
            }
            // Sign in the user with this external login provider if the user already has a login
            //var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        TempData["LinkId"] = loginInfo.Login.ProviderKey;
            //        TempData["LinkType"] = loginInfo.Login.LoginProvider;
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
            //    case SignInStatus.Failure:
            //    default:
            //        // If the user does not have an account, then prompt the user to create an account
            //        ViewBag.ReturnUrl = returnUrl;
            //        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, LinkId = loginInfo.Login.ProviderKey });
            //}
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                var authenticationManager = HttpContext.GetOwinContext().Authentication;
                // Get the information about the user from the external login provider
                var loginInfo = await authenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    return View("ExternalLoginFailure");
                }

                //var authenticatedUser = UserRepo.GetByLinkId(model.LinkId);
                //if (authenticatedUser == null)
                //{
                //    UserLinkedAccountTypeEnums type = UserLinkedAccountTypeEnums.Email;
                //    switch (info.Login.LoginProvider.ToLower())
                //    {
                //        case "facebook": type = UserLinkedAccountTypeEnums.Facebook; break;
                //        case "google": type = UserLinkedAccountTypeEnums.Gmail; break;
                //        default: break;
                //    }

                //    var newUser = UserRepo.RegisterWebsite(model.LinkId, model.Email, type);
                //}    var authenticationManager = HttpContext.GetOwinContext().Authentication;

                TempData["LinkId"] = loginInfo.Login.ProviderKey;
                TempData["LinkType"] = loginInfo.Login.LoginProvider;


                var type = UserLinkedAccountTypeEnums.Facebook;
                if (loginInfo.Login.LoginProvider.ToLower().Contains("facebook"))
                {
                    type = UserLinkedAccountTypeEnums.Facebook;
                }
                else if (loginInfo.Login.LoginProvider.ToLower().Contains("google"))
                {
                    type = UserLinkedAccountTypeEnums.Gmail;
                }

                var res = AuthenticationRepo.AuthenticateLink(new LoginRequest
                {
                    LinkId = loginInfo.Login.ProviderKey,
                    Type = type,    
                    Name = loginInfo.ExternalIdentity.Name,
                    Email = loginInfo.Email,
                    Description = loginInfo.Email
                });
           

                if (res.Success)
                {
                    WebFormsAuthenticationService.CreateCookie(res.ReturnData, false);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, LinkId = loginInfo.Login.ProviderKey });
                }
                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                //return RedirectToLocal(returnUrl);

                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //var result = await UserManager.CreateAsync(user);
                //if (result.Succeeded)
                //{
                //    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                //    if (result.Succeeded)
                //    {
                //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //        return RedirectToLocal(returnUrl);
                //    }
                //}
                //AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [AllowAnonymous]
     //   [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebFormsAuthenticationService.Signout();
            Session.Abandon();
            Session.RemoveAll();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public ActionResult Logout()
        {
            WebFormsAuthenticationService.Signout();
            Session.Abandon();
            Session.RemoveAll();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    if (_userManager != null)
            //    {
            //        _userManager.Dispose();
            //        _userManager = null;
            //    }

            //    if (_signInManager != null)
            //    {
            //        _signInManager.Dispose();
            //        _signInManager = null;
            //    }
            //}

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

   

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}