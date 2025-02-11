using Project.Database.DbContext;
using Project.Models.API.Authentication;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using static Project.Models.Enums;
using Infrastructure;
using Infrastructure.Random;
using Project.Repository.Repo.Sender;

namespace Project.Repository.Repo
{
    public class AuthenticationRepo
    {
        public static ReturnValue SendMobileVerificationCode(string mobile)
        {
            using (var context = new LakbayDBEntities())
            {
                var code = RandomString.RandomNumber(6);
                SenderRepo.AddQueueSms($"Your verification code is {code}", mobile);

                return new ReturnValue { Success = true, ReturnParam = code, Message = "Verification code has been sent to your mobile number" };
            }
        }
        public static ReturnValue SendEmailVerificationCode(string email)
        {
            using (var context = new LakbayDBEntities())
            {
                var code = RandomString.RandomNumber(6);
                SenderRepo.SendCodeEmail(code, email, "Verification Code", EmailType.VerificationCode);

                return new ReturnValue { Success = true, ReturnParam = code, Message = "Verification code has been sent to your email" };
            }
        }


        private static bool CanCreateUser(UserLinkedAccountTypeEnums type)
        {
            if (type == UserLinkedAccountTypeEnums.Mobile || type == UserLinkedAccountTypeEnums.Email)
                return false;
            return true;
        }

        public static ReturnValue<MobileAuthUserModel> GetUser(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                return new ReturnValue<MobileAuthUserModel> { Success = true, ReturnData = GetUser(context, id) };
            }
        }

        public static ReturnValue<AuthUserModel> AuthenticateLink(LoginRequest req)
        {
            using (var context = new LakbayDBEntities())
            {
                var link = context.UserLinkedAccounts.Include(s => s.User).FirstOrDefault(s => s.Type == (int)req.Type && s.LinkId == req.LinkId);
                var user = link?.User;// ?? context.UserDetails.Include(s => s.User).FirstOrDefault(s => s.Email == req.UsernameOrEmail)?.User;

                if (CanCreateUser(req.Type) && user == null) // apple google fb
                {
                    return CreateUser(context, req);
                }
                else if (user == null)
                {
                    return new ReturnValue<AuthUserModel> { Message = "Invalid username or password" };
                }
                else
                {
                    user = context.Users.Include(s => s.UserDetails).Include(s => s.UserLinkedAccounts).FirstOrDefault(s => s.Id == user.Id);
                    if (req.Type == UserLinkedAccountTypeEnums.Mobile || req.Type == UserLinkedAccountTypeEnums.Email)
                    {
                        var dec = Fletcher.Decrypt(user.Password, user.PasswordKey);
                        var encrypt = Fletcher.Encrypt(req.Password, user.PasswordKey);
                        if (encrypt != user.Password)
                        {
                            return new ReturnValue<AuthUserModel> { Message = "Invalid username or password" };
                        }
                    }


                    SaveLink(context, user, req);
                    return new ReturnValue<AuthUserModel> { Success = true, ReturnData = GetUser(context, user.Id) };
                }
            }
        }
        public static bool Authenticate(int userId, string password)
        {
            using (var context = new LakbayDBEntities())
            {
                var user = context.Users.FirstOrDefault(a => a.Id == userId);

                if (user != null)
                {
                    var encryptedPassword = Fletcher.Encrypt(password, user.PasswordKey);

                    if (encryptedPassword == user.Password)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public static ReturnValue Authenticate(string username, string password)
        {
            var result = new ReturnValue() { Message = "Invalid Username!" };
            using (var context = new LakbayDBEntities())
            {
                var user = context.Users.FirstOrDefault(a => a.Username == username && a.IsAdmin);

                if (user != null)
                {
                    var encryptedPassword = Fletcher.Encrypt(password, user.PasswordKey);

                    if (encryptedPassword == user.Password)
                    {
                        var userDetail = context.UserDetails.FirstOrDefault(a => a.UserId == user.Id);

                        var model = new AuthUserModel()
                        {
                            Id = user.Id,
                            Email = userDetail?.Email ?? "",
                            FirstName = userDetail?.FullName ?? "",
                            Guid = user.GUID,
                            Phone = userDetail?.Mobile ?? "",
                            ProfilePicturePath = userDetail?.ProfilePictureUrl ?? "",
                            IsAdmin = user.IsAdmin
                        };

                        result.Message = "Welcome " + model.FirstName + "!";
                        result.Success = true;
                        result.ReturnParam = model;
                    }
                    else
                    {
                        result.Message = "Username and password are incorrect.";
                    }
                }
            }

            return result;
        }

        public static ReturnValue ChangePassword(ChangePasswordModel pass)
        {
            using (var context = new LakbayDBEntities())
            {
                var user = context.Users.FirstOrDefault(s => s.Id == pass.UserId);
                if (user != null)
                {
                    var password = Fletcher.Encrypt(pass.Password, user.PasswordKey);
                    if (password != user.Password && pass.AuthenticateOldPassword)
                    {
                        return new ReturnValue { Message = "Password incorrect" };
                    }
                    user.Password = Fletcher.Encrypt(pass.NewPassword, user.PasswordKey);

                    var suc = context.SaveChanges() > 0;
                    if (suc)
                    {
                        return new ReturnValue { Success = true, Message = "Password has been changed" };
                    }
                    else
                    {
                        return new ReturnValue { Message = "No changes has been made." };
                    }
                }
                return new ReturnValue { Message = "Please try again later" };
            }
        }

        public static ReturnValue SetPassword(int userId, string password)
        {
            using (var context = new LakbayDBEntities())
            {
                var user = context.Users.FirstOrDefault(s => s.Id == userId);
                if (user != null)
                {
                    user.Password = Fletcher.Encrypt(password, user.PasswordKey);

                    var suc = context.SaveChanges() > 0;
                  
                    return new ReturnValue { Success = true, Message = "Password has been changed" };
                  
                }
            }
            return new ReturnValue {  };
        }

        private static ReturnValue<AuthUserModel> CreateUser(LakbayDBEntities context, LoginRequest req)
        {
            var res = UserRepo.CreateUser(context,
                                email: req.Email,
                                name: req.Name,
                                mobile: req.Mobile,
                                profilePicURL: req.ProfilePicURL,
                                linkType: req.Type,
                                linkDisplay: req.Description,
                                linkId: req.LinkId,
                                req.Password);
            if (res.Success)
            {
                return new ReturnValue<AuthUserModel> { Success = true, ReturnData = GetUser(context, (int)res.ReturnParam) };
            }
            else
            {
                return new ReturnValue<AuthUserModel> { Message = "Try again later" };
            }
        }

        private static void SaveLink(LakbayDBEntities context, User user, LoginRequest req)
        {
            var link = user.UserLinkedAccounts.FirstOrDefault(s => s.UserId == user.Id &&
                                                                   s.LinkId == req.LinkId &&
                                                                   s.Type == (int)req.Type);

            if (link == null)
            {
                user.UserLinkedAccounts.Add(new UserLinkedAccount
                {
                    DateLinked = DateTime.Now,
                    LinkId = req.LinkId,
                    Type = (int)req.Type,
                    Description = req.Description
                });
            }
            else if (link != null && !string.IsNullOrEmpty(req.Description) && link.Description != req.Description) 
            {
                link.Description = req.Description;
            }
            context.SaveChanges();
        }

        private static MobileAuthUserModel GetUser(LakbayDBEntities context, int userId)
        {
            var user = context.Users.Include(s => s.UserDetails).Include(s => s.UserLinkedAccounts).FirstOrDefault(s => s.Id == userId);
            var details = user.UserDetails.FirstOrDefault();
            var links = user.UserLinkedAccounts;

            var model = new MobileAuthUserModel();
            model.Guid = user.GUID;
            model.Email = links.FirstOrDefault(s => s.Type == (int)UserLinkedAccountTypeEnums.Email && (s.IsVerified ?? false))?.LinkId;
            model.FirstName = details.FullName;
            model.Id = user.Id;
            model.Phone = details.Mobile;
            model.AppleId = links.FirstOrDefault(s => s.Type == (int)UserLinkedAccountTypeEnums.Apple)?.LinkId;
            model.GmailId = links.FirstOrDefault(s => s.Type == (int)UserLinkedAccountTypeEnums.Gmail)?.LinkId;
            model.FacebookId = links.FirstOrDefault(s => s.Type == (int)UserLinkedAccountTypeEnums.Facebook)?.LinkId;
            model.IsAdmin = user.IsAdmin;
            return model;
        }
    }
}
