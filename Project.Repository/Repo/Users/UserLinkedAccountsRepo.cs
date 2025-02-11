using Infrastructure;
using Infrastructure.Random;
using Project.Database.DbContext;
using Project.Models.Models;
using Project.Models.Models.UserModels;
using Project.Repository.Repo.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using static Project.Models.Enums;
using Project.Models.API.User;

namespace Project.Repository.Repo.Users
{
    public class UserLinkedAccountsRepo
    {
        public static UserLinkedAccountDto ToDto(UserLinkedAccount ent)
        {
            if (ent == null) return null;
            return new UserLinkedAccountDto
            {
                Id = ent.Id,
                UserId = ent.UserId,
                LinkId = ent.LinkId,
                Description = ent.Description,
                Type = (Models.Enums.UserLinkedAccountTypeEnums)ent.Type,
                IsVerified = ent.IsVerified
            };
        }
        public static UserLinkedAccount ToEntity(UserLinkedAccountDto dto)
        {
            if (dto == null) return null;
            return new UserLinkedAccount
            {
                Id = dto.Id,
                UserId = dto.UserId,
                LinkId = dto.LinkId,
                Description = dto.Description,
                Type = (int)dto.Type,
                DateLinked = DateTime.Now,
                IsVerified = dto.IsVerified
            };
        }

       
        private static bool IsNeedToCreatePassword (LakbayDBEntities context, int userId)
        {
            var isExisting = context.UserLinkedAccounts.Any(s => s.UserId == userId &&
                                                              (s.Type == (int)UserLinkedAccountTypeEnums.Email ||
                                                              s.Type == (int)UserLinkedAccountTypeEnums.Mobile));
            if(!isExisting)
            {
                return true;
            }
            return false;
        }
       

        public static ReturnValue IsAccountAvailable(string linkId, UserLinkedAccountTypeEnums type)
        {
            using (var context = new LakbayDBEntities())
            {
                var link = context.UserLinkedAccounts.FirstOrDefault(s => s.LinkId == linkId && s.Type == (int)type);
                return new ReturnValue { Success = link == null ? true : false };
            }
        }


        public static bool NeedToCreatePassword(int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                return IsNeedToCreatePassword(context, userId);
            }
        }

        public static ReturnValue<AddLinkedAccountResponse> Add(UserLinkedAccountDto dto)
        {
            using(var context = new LakbayDBEntities())
            {
                var link = context.UserLinkedAccounts.FirstOrDefault(s => s.LinkId == dto.LinkId && s.Type == (int)dto.Type);
                if(link != null)
                {
                    return new ReturnValue<AddLinkedAccountResponse> { Message = "Account is already registered " };
                }

                var ownlink = context.UserLinkedAccounts.FirstOrDefault(s => s.UserId == dto.UserId && s.Type == (int)dto.Type);
                if(ownlink != null)
                {
                    return new ReturnValue<AddLinkedAccountResponse> { Message = $"Account already have a {dto.Type} linked account " };
                }
                var needToCreatePassword = IsNeedToCreatePassword(context, dto.UserId);

                context.UserLinkedAccounts.Add(ToEntity(dto));
                var res = context.SaveChanges() > 0;
                if(res)
                {
                    //if(!string.IsNullOrEmpty(hasExisting))
                    //{
                    //    var user = context.Users.Include(s => s.UserDetails).FirstOrDefault(s => s.Id == dto.UserId);
                    //    var tempPassword = hasExisting;
                    //    var message = string.Format("Your temporary password is {0}", tempPassword);
                    //    if (dto.Type == UserLinkedAccountTypeEnums.Email)
                    //    {
                    //        SenderRepo.SendCodeEmail(tempPassword, dto.LinkId, "Temporary Password", Sender.EmailType.ForgotPassword);
                    //    }

                    //    else if (dto.Type == UserLinkedAccountTypeEnums.Mobile)
                    //        SenderRepo.AddQueueSms(message, dto.LinkId);
                    //    var ret = context.UserLinkedAccounts.FirstOrDefault(s => s.LinkId == dto.LinkId && s.Type == (int)dto.Type);
                    //    return new ReturnValue<UserLinkedAccountDto> { Success = true, Message = "Account linked. Temporary password has been sent to your mobile/email", ReturnData = ToDto(ret) };
                    //}
                    //else
                    {
                        var ret = context.UserLinkedAccounts.FirstOrDefault(s => s.LinkId == dto.LinkId && s.Type == (int)dto.Type);
                        return new ReturnValue<AddLinkedAccountResponse> { Success = true, Message = "Account linked", 
                            ReturnData = new AddLinkedAccountResponse
                            {
                                Account = ToDto(ret),
                                NeedToCreatePassword = needToCreatePassword
                            }
                        };
                    }
                }
                else
                {
                    return new ReturnValue<AddLinkedAccountResponse> { Message = "Failed to save" };
                }
            }
        }


        public static ReturnValue Delete(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                var link = context.UserLinkedAccounts.FirstOrDefault(s => s.Id == id);
                if (link != null)
                {
                    var userLinkedAccounts = context.UserLinkedAccounts.Where(s => s.UserId == link.UserId);
                    if(userLinkedAccounts.Count() == 1)
                    {
                        return new ReturnValue { Message = "Cannot remove last linked account" };
                    }

                    context.UserLinkedAccounts.Remove(link);

                    //remove password if no email or mobie
                    var userLinkedAccounts2 = context.UserLinkedAccounts.Where(s => s.UserId == link.UserId && 
                                                                                    (s.Type == (int)UserLinkedAccountTypeEnums.Mobile ||
                                                                                     s.Type == (int)UserLinkedAccountTypeEnums.Email) &&
                                                                                    s.Id != link.Id).ToList();
                    if(userLinkedAccounts2.Count() == 0)
                    {
                        var user = context.Users.FirstOrDefault(s => s.Id == link.UserId);
                        user.Password = "";
                    }
                    //

                    var res = context.SaveChanges() > 0;
                    if (res)
                    {
                        return new ReturnValue { Success = true, Message = "Linked account removed" };
                    }
                    else
                    {
                        return new ReturnValue { Message = "Failed to remove linked account" };
                    }
                }

                return new ReturnValue {  Message = "Account not found" };
            }
        }
        public static UserLinkedAccountDto Get(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                return ToDto(context.UserLinkedAccounts.FirstOrDefault(s => s.Id == id));
            }
        }
        public static IEnumerable<UserLinkedAccountDto> GetList(int userId)
        {
            using (var context = new LakbayDBEntities())
            {
                return context.UserLinkedAccounts.Where(s => s.UserId == userId).ToList().Select(s => ToDto(s));
            }
        }

        public static bool? IsVerified(int userId, UserLinkedAccountTypeEnums type)
        {
            using (var context = new LakbayDBEntities())
            {
                var linkedAccount = context.UserLinkedAccounts.FirstOrDefault(s => s.UserId == userId && s.Type == (int)type);
                
                return linkedAccount?.IsVerified;
            }
        }

        public static ReturnValue VerifyLinkedAccount(int id)
        {
            using (var context = new LakbayDBEntities())
            {
                var linkedAccount = context.UserLinkedAccounts.FirstOrDefault(s => s.Id == id);

             
                linkedAccount.IsVerified = true;

                var suc = context.SaveChanges() > 0;
                if(suc)
                {
                    return new ReturnValue { Success = true, Message = "Account Verified" };
                }
                else
                {
                    return new ReturnValue { Message = "Please try again later" };
                }
            }
        }
    }
}
