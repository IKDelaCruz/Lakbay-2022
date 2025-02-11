using AutoMapper;
using Project.Database.DbContext;
using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Infrastructure.Random;
using Infrastructure;
using Project.Models.API;
using Project.Repository.Repo.Sender;
using static Project.Models.Enums;
using Project.Models;
using Project.Models.Enums_;
using System.IO;
using Infrastructure.Files;

namespace Project.Repository.Repo
{
    public class UserRepo
    {
        public static UserDto ToDto(User ent)
        {
            if (ent == null) return null;
            return new UserDto
            {
                Id = ent.Id,
                IsDeleted = ent.IsDeleted,
                Username = ent.Username,
                GUID = ent.GUID
            };
        }
        //public static User ToEnt(UserDto dto, User ent = null)
        //{
        //    ent = ent ?? new User();
        //    ent.DateCreated = dto.DateCreated;
        //    ent.DateDeleted = dto.DateDeleted;
        //    ent.IsDeleted = dto.IsDeleted;
        //    ent.IsLocked = dto.IsLocked;
        //    ent.LastLoginAttempt = dto.LastLoginAttempt;
        //    ent.LastLoginDate = dto.LastLoginDate;
        //    ent.LoginCounter = dto.LoginCounter;
        //    ent
        //}

        public static ReturnValue<MobileAuthUserModel> EmailMobileRegistration(UserRegistrationDto reg)
        {
            using (var context = new LakbayDBEntities())
            {
                var user = CreateUser(context,
                                  email: reg.Email ?? "",
                                  name: reg.FullName ?? "",
                                  mobile: reg.MobileNumber ?? "",
                                  profilePicURL: "",
                                  linkType: reg.RegistrationType,
                                  linkDisplay: reg.LinkId,
                                  linkId: reg.LinkId,
                                  reg.Password,
                                  city: reg.City,
                                  country: reg.Country,
                                  gender: reg.Gender,
                                  age: reg.Age);


                if (user.Success)
                {
                    return AuthenticationRepo.GetUser((int)user.ReturnParam);
                }
                else
                {
                    return new ReturnValue<MobileAuthUserModel> { Success = false, Message = user.Message };
                }
            }
        }

        public static ReturnValue<AuthenticatedUserModel> RegisterWebsite(string linkid, string email, UserLinkedAccountTypeEnums linkType)
        {
            using (var context = new LakbayDBEntities())
            {
                var user = CreateUser(context,
                                  email: email,
                                  name: "",
                                  mobile: "",
                                  profilePicURL: "",
                                  linkType: linkType,
                                  linkDisplay: linkid,
                                  linkId: linkid,
                                  "");


                if (user.Success)
                {
                    return new ReturnValue<AuthenticatedUserModel> { Success = true, Message = user.Message, ReturnData = GetByEmail(email) };
                }
                else
                {
                    return new ReturnValue<AuthenticatedUserModel> { Success = false, Message = user.Message };
                }
            }
        }

        public static ReturnValue ForgotPassword(string username)
        {
            var result = new ReturnValue();

            using (var context = new LakbayDBEntities())
            {
                var linkedAccount = context.UserLinkedAccounts.Include(s => s.User)
                                                      .FirstOrDefault(s => s.LinkId == username &&
                                                       (s.Type == (int)UserLinkedAccountTypeEnums.Email || s.Type == (int)UserLinkedAccountTypeEnums.Mobile));
                User user = linkedAccount?.User;
                if (user == null)
                {
                    //do not show entered email/number is invalid
                    result.Message = "Temporary password has been sent to your email/mobile.";
                }
                else
                {
                    var tempPassword = RandomString.RandomTextNumbers(6);
                    var password = Fletcher.Encrypt(tempPassword, user.PasswordKey);
                    user.Password = password;

                    if (context.SaveChanges() > 0)
                    {
                        var message = string.Format("Your temporary password is {0}", tempPassword);

                        if (linkedAccount.Type == (int)UserLinkedAccountTypeEnums.Email)
                        {
                            SenderRepo.SendCodeEmail(tempPassword, linkedAccount.LinkId, "Temporary Password", Sender.EmailType.ForgotPassword);
                        }

                        if (linkedAccount.Type == (int)UserLinkedAccountTypeEnums.Mobile)
                            SenderRepo.AddQueueSms(message, linkedAccount.LinkId);

                        result.Message = "Temporary Password has been sent to your email/mobile.";
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = "Unable to update your password at this time.";
                    }
                }
            }

            return result;
        }
        public static ReturnValue IsLinkExisting(UserLinkedAccountTypeEnums linkType, string linkId)
        {
            using(var context = new LakbayDBEntities())
            {
                var tple = IsLinkExisting(context, linkType, linkId);
                return new ReturnValue
                {
                    ReturnParam = tple.Item1,
                    Message = tple.Item2
                };
            }
         
        }
        private static Tuple<bool, string> IsLinkExisting(LakbayDBEntities context, UserLinkedAccountTypeEnums linkType, string linkId)
        {
            var item = context.UserLinkedAccounts.FirstOrDefault(s => s.LinkId == linkId && s.Type == (int)linkType);

            if (item == null)
                return new Tuple<bool, string>(false, "");

            switch (linkType)
            {
                case UserLinkedAccountTypeEnums.Mobile:
                    return new Tuple<bool, string>(true, "Mobile number is already used");
                case UserLinkedAccountTypeEnums.Email:
                    return new Tuple<bool, string>(true, "Email is already used");
                case UserLinkedAccountTypeEnums.Gmail:
                    return new Tuple<bool, string>(true, "Google account is already linked");
                default:
                    return new Tuple<bool, string>(true, $"{linkType.ToString()} account is already linked");
            }
        }

        public static ReturnValue CreateUser(LakbayDBEntities context,
          string email, string name, string mobile, string profilePicURL, UserLinkedAccountTypeEnums linkType,
          string linkDisplay, string linkId, string password, string country = "", string city = "", Gender? gender = null, int? age = null)
        {
            var isExisting = IsLinkExisting(context, linkType, linkId);
            if (isExisting.Item1)
            {
                return new ReturnValue { Message = isExisting.Item2 };
            }


            var passwordKey = Guid.NewGuid().ToString();
            //var password = RandomString.RandomNumber(6).ToLower();
            var encryptedPassword = "";
            if (!string.IsNullOrEmpty(password))
                encryptedPassword = Fletcher.Encrypt(password, passwordKey);
            //var username = RandomString.RandomText(7).ToLower();
            var user = new User()
            {
                DateCreated = DateTime.Now,
                Password = encryptedPassword,
                PasswordKey = passwordKey,
                Username = RandomString.RandomText(7).ToLower(),
                GUID = Guid.NewGuid().ToString()
            };
            user.UserDetails.Add(new UserDetail
            {
                Address = "",
                Email = email ?? "",
                FullName = name ?? "",
                IsDeleted = false,
                Mobile = mobile ?? "",
                ProfilePictureUrl = profilePicURL ?? "",
                Gender = (int?)gender ?? -1,
                Country = country,
                City = city,
                Age = age
            });

            bool? isVerified = null;
            if (linkType == UserLinkedAccountTypeEnums.Email || linkType == UserLinkedAccountTypeEnums.Mobile)
                isVerified = false;
            user.UserLinkedAccounts.Add(new UserLinkedAccount
            {
                LinkId = linkId,
                DateLinked = DateTime.Now,
                Type = (int)linkType,
                Description = linkDisplay,
                IsVerified = isVerified
            });

            context.Users.Add(user);
            var res = context.SaveChanges() > 0;

            if (res)
            {
                return new ReturnValue { Success = true, Message = "Signed up successfully!", ReturnParam = user.Id };
            }
            else
            {
                return new ReturnValue { Message = "Try again later" };
            }

        }

        public static UserDto Get(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                return ToDto(context.Users.FirstOrDefault(s => !s.IsDeleted && s.Id == id));
            }
        }

        public static ReturnValue GetByUsername(string username)
        {
            var result = new ReturnValue() { Message = "Invalid Details!" };
            using (var context = new LakbayDBEntities())
            {
                var user = context.Users.FirstOrDefault(a => !a.IsDeleted && a.Username == username);

                if (user != null)
                {
                    var userDetail = context.UserDetails.FirstOrDefault(a => a.UserId == user.Id);

                    var model = new MobileAuthUserModel()
                    {
                        Id = user.Id,
                        Email = userDetail?.Email ?? "",
                        FirstName = userDetail?.FullName ?? "",
                        Guid = user.GUID,
                        Phone = userDetail?.Mobile ?? "",
                        ProfilePicturePath = userDetail?.ProfilePictureUrl ?? "",
                        Country = userDetail?.Country ?? "",
                        City = userDetail?.City ?? "",
                        Gender = userDetail != null && userDetail.Gender == (int)Gender.Female ? Gender.Female.ToString() : Gender.Male.ToString(),
                        Age = userDetail?.Age ?? 0,
                    };

                    result.Message = "Welcome " + model.FirstName + "!";
                    result.Success = true;
                    result.ReturnParam = model;
                }
            }

            return result;
        }

        public static UserDto Get(string guid)
        {
            using (var context = new LakbayDBEntities())
            {
                return ToDto(context.Users.FirstOrDefault(s => !s.IsDeleted && s.GUID == guid));
            }
        }
        public static ReturnValue GetUserByQR(string guid)
        {
            var result = new ReturnValue() { Message = "Invalid Details!" };
            using (var context = new LakbayDBEntities())
            {
                var user = context.Users.FirstOrDefault(a => !a.IsDeleted && a.GUID == guid);

                if (user != null)
                {
                    var userDetail = context.UserDetails.FirstOrDefault(a => a.UserId == user.Id);

                    var model = new MobileAuthUserModel()
                    {
                        Id = user.Id,
                        Email = userDetail?.Email ?? "",
                        FirstName = userDetail?.FullName ?? "",
                        Guid = user.GUID,
                        Phone = userDetail?.Mobile ?? "",
                        ProfilePicturePath = userDetail?.ProfilePictureUrl ?? "",
                        Country = userDetail?.Country ?? "",
                        City = userDetail?.City ?? "",
                        Gender = userDetail != null && userDetail.Gender == (int)Gender.Female ? Gender.Female.ToString() : Gender.Male.ToString(),
                        Age = userDetail?.Age ?? 0,
                    };

                    result.Message = "Welcome " + model.FirstName + "!";
                    result.Success = true;
                    result.ReturnParam = model;
                }
            }

            return result;
        }
        public static AuthenticatedUserModel GetByLinkId(string linkid)
        {
            using (var context = new LakbayDBEntities())
            {
                var link = context.UserLinkedAccounts.Where(a => a.LinkId == linkid).Include(b => b.User)
                                       .Where(c => !c.User.IsDeleted).FirstOrDefault();

                if (link == null)
                    return null;

                if (link != null && link.User == null)
                    return null;

                var detail = context.UserDetails.FirstOrDefault(a => a.UserId == link.User.Id);

                return new AuthenticatedUserModel()
                {
                    Id = detail?.Id ?? 0,
                    UserId = link.User.Id,
                    FullName = detail?.FullName ?? "",
                    Age = detail?.Age ?? 0,
                    Gender = detail != null ? (Gender)(detail.Gender) : Gender.Male,
                    City = detail?.City ?? "",
                    Country = detail?.Country ?? "",
                    Email = detail?.Email ?? "",
                    Mobile = detail?.Mobile ?? "",
                    Address = detail?.Address ?? "",
                    IsDeleted = link.User.IsDeleted,
                    ProfilePictureUrl = detail?.ProfilePictureUrl ?? "",
                    Username = link.User.Username
                };
            }
        }
        public static AuthenticatedUserModel GetByEmail(string email)
        {
            using (var context = new LakbayDBEntities())
            {
                var detail = context.UserDetails.Where(a => a.Email == email).Include(b => b.User)
                                    .Where(c => !c.IsDeleted).FirstOrDefault();

                if (detail == null)
                    return null;

                if (detail != null && detail.User == null)
                    return null;

                return new AuthenticatedUserModel()
                {
                    Id = detail.Id,
                    UserId = detail.UserId,
                    FullName = detail.FullName,
                    Age = detail.Age,
                    Gender = (Gender)(detail.Gender),
                    City = detail.City,
                    Country = detail.Country,
                    Email = detail.Email,
                    Mobile = detail.Mobile,
                    Address = detail.Address,
                    IsDeleted = detail.User.IsDeleted,
                    ProfilePictureUrl = detail.ProfilePictureUrl,
                    Username = detail.User.Username
                };
            }
        }




        private static bool HasSameEmail(LakbayDBEntities context, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            var sameEmail = context.Users.Include(s => s.UserDetails)
                                         .Where(s => !s.IsDeleted)
                                         .Where(s => s.UserDetails.FirstOrDefault().Email == email)
                                         .FirstOrDefault();
            return sameEmail != null;
        }
        private static ReturnValue IsUsernameValid(string username)
        {
            //if (string.IsNullOrEmpty(username))
            //{
            //    return new ReturnValue { Message = "Please enter a username " };
            //}

            //if (!username.Any(char.IsLetter))
            //{
            //    return new ReturnValue { Message = "Username must include at least one alphabetical character (a - z)" };
            //}

            //if (username.Contains('@'))
            //{
            //    return new ReturnValue { Message = "Username must not include the '@' character" };
            //}

            return new ReturnValue { Success = true };
        }

        public static string GetProfilePicturePath(string defaultIconPath, UserDto user, bool isThumbnail)
        {
            var image = UserDetailRepo.GetProfileImage(user?.Id ?? 0);
            var guid = user.GUID;
            if (image == null)
            {
                return defaultIconPath;
            }


            var filePath = "";
            if (isThumbnail)
            {
                filePath = FileManager.GetVirtualPath(Path.Combine(UserDetailRepo.GetProfilePictureFolderPath(guid), image.ThumbnailSystemName));
                if (!FileManager.IsFileExists(filePath))
                    filePath = FileManager.GetVirtualPath(Path.Combine(UserDetailRepo.GetProfilePictureFolderPath(guid), image.SystemName));
            }
            else
            {
                filePath = FileManager.GetVirtualPath(Path.Combine(UserDetailRepo.GetProfilePictureFolderPath(guid), image.SystemName));
            }

            if (!FileManager.IsFileExists(filePath))
            {
                return defaultIconPath;
            }
            else
            {
                return filePath;
            }
        }
    }
}
