using Microsoft.Owin.Security;
using Project.Models.API.Authentication;
using Project.Models.Models;
using Project.Models.Models.UserModels;
using Project.Repository.Repo;
using Project.Repository.Repo.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web.Models;
using Web.ViewModel.Profile;
using static Project.Models.Enums;

namespace Web.Controllers
{
    public class LinkedAccountsController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            var typeList = Enum.GetValues(typeof(UserLinkedAccountTypeEnums)).Cast<UserLinkedAccountTypeEnums>();

            var linkedAccounts = UserLinkedAccountsRepo.GetList(UserId);

            var list = typeList.Select(s => new LinkedAccountViewModel
            {
                Type = s,
                dto = linkedAccounts.FirstOrDefault(q => q.Type == s) ?? new Project.Models.Models.UserModels.UserLinkedAccountDto { Type = s }
            });
            
            if (!string.IsNullOrEmpty(TempData["message"]?.ToString()))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            return View(list);
        }


        [Authorize]
        [HttpPost]
        public JsonResult SendCode(string username, UserLinkedAccountTypeEnums type)
        {
            var retVal = new ReturnValue();
            if (type == UserLinkedAccountTypeEnums.Email)
                retVal = AuthenticationRepo.SendEmailVerificationCode(username);
            else
                retVal = AuthenticationRepo.SendMobileVerificationCode(username);
            var jResult = Json(new
            {
                returnData = retVal
            }, JsonRequestBehavior.AllowGet);
            jResult.MaxJsonLength = int.MaxValue;
            return jResult;
        }

        [Authorize]
        public ActionResult Verify(AddLinkedAccountViewModel vm)
        {          
            return View(new VerifyViewModel
            {
                LinkedAccount = vm,
                hiddenVerifyCode = ""
            });
        }
        [Authorize]
        [HttpPost]
        public ActionResult Verify(VerifyViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.VerifyCode))
            {
                ViewBag.Message = "Please enter the verification code";
                vm.VerifyCode = "";
                return View(vm);
            }
            if(vm.VerifyCode != vm.hiddenVerifyCode)
            {
                ViewBag.Message = "Incorrect verification code";
                vm.VerifyCode = "";
                return View(vm);
            }
            else
            {
                var needCreatePassword = UserLinkedAccountsRepo.NeedToCreatePassword(UserId);
                if (needCreatePassword)
                {
                    return RedirectToAction("CreatePassword", new RouteValueDictionary(vm.LinkedAccount));
                }
                else
                {
                    var dto = new UserLinkedAccountDto()
                    {
                        UserId = UserId,
                        Description = vm.LinkedAccount.GetUsername(),
                        Type = vm.LinkedAccount.Type,
                        LinkId = vm.LinkedAccount.GetUsername()
                    };
                    var result = UserLinkedAccountsRepo.Add(dto);
                    if (result.Success)
                    {
                        TempData["message"] = result.Message;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = result.Message;
                        return View(vm);
                    }
                }
            }
        }


        [Authorize]
        public ActionResult CreatePassword(AddLinkedAccountViewModel vm)
        {
            return View(new CreatePasswordViewModel
            {
                LinkedAccount = vm
            });
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreatePassword(CreatePasswordViewModel vm)
        {
            if (vm.NewPassword != vm.ConfirmPassword)
            {
                ViewBag.Message = "Pssword does not match confirm password";
                vm.NewPassword = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.NewPassword))
            {
                ViewBag.Message = "Please enter a password";
                vm.NewPassword = "";
                vm.ConfirmPassword = "";
                return View(vm);
            }

            var change = AuthenticationRepo.SetPassword(UserId, vm.NewPassword);
            if (change.Success)
            {
                var dto = new UserLinkedAccountDto()
                {
                    UserId = UserId,
                    Description = vm.LinkedAccount.GetUsername(),
                    Type = vm.LinkedAccount.Type,
                    LinkId = vm.LinkedAccount.GetUsername()
                };
                var result = UserLinkedAccountsRepo.Add(dto);
                if (result.Success)
                {
                    TempData["message"] = result.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = result.Message;
                    return View(vm);
                }
            }
            else
            {
                ViewBag.Message = change.Message;
                return View(vm);
            }
        }

        [Authorize]
        public ActionResult LinkAccount(UserLinkedAccountTypeEnums type)
        {            
            return View(new AddLinkedAccountViewModel()
            {
                Type = type
            });
        }

        [Authorize]
        [HttpPost]
        public ActionResult LinkAccount(AddLinkedAccountViewModel vm)
        {
            var isAvailable = UserLinkedAccountsRepo.IsAccountAvailable(vm.GetUsername(), vm.Type);
            if(!isAvailable.Success)
            {
                ViewBag.Message = $"{vm.Type} is already taken";
                return View(vm);
            }
            return RedirectToAction("Verify", new RouteValueDictionary(vm));
        }


        [Authorize]
        [HttpPost]
        public ActionResult Unlink(int id)
        {
            var link = UserLinkedAccountsRepo.Get(id);
            if(link == null || UserId != link.UserId)
            {
                var jResult = Json(new
                {
                    success = false,
                    returnData = new ReturnValue { Message = "Please refresh the page" }
                }, JsonRequestBehavior.AllowGet) ;
                jResult.MaxJsonLength = int.MaxValue;
                return jResult;
            }
            else
            {
                var result = UserLinkedAccountsRepo.Delete(id);
                var jResult = Json(new
                {
                    success = result.Success,
                    returnData = result
                }, JsonRequestBehavior.AllowGet) ;
                jResult.MaxJsonLength = int.MaxValue;
                return jResult;
            }
        }



        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public ActionResult ExternalLink(string provider, string returnUrl)
        {      
            var url = Url.Action("ExternalLinkCallback", "LinkedAccounts", new { ReturnUrl = returnUrl });
            return new ChallengeResult(provider, url);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLinkCallback(string returnUrl)
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
            else if (loginInfo.Login.LoginProvider.ToLower().Contains("google"))
            {
                type = UserLinkedAccountTypeEnums.Gmail;
            }

            var res = UserLinkedAccountsRepo.Add(new UserLinkedAccountDto
            {
                LinkId = loginInfo.Login.ProviderKey,
                Type = type,
                Description = loginInfo.Email,
                UserId = UserId
            });

            if (res.Success)
            {
                return RedirectToLocal(returnUrl);
            }
            else
            {
                TempData["message"] = res.Message;
                return RedirectToAction("Index");
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
            private const string XsrfKey = "XsrfId";
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

    }
}