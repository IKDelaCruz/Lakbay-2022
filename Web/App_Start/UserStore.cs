//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Project.Models;
//using Project.Repository.Repo.Users;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace Web.App_Start
//{
//    public class UserStore<TUser> :
//        IUserStore<TUser>,
//        IUserPasswordStore<TUser>,
//        IUserLoginStore<TUser>,
//        IUserRoleStore<TUser>
//        where TUser : IdentityUser
//    {
//        private readonly Func<string, string> _partitionKeyFromId;
//        private readonly CloudTable _userTableReference;
//        private readonly CloudTable _loginTableReference;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="connectionString">Storage connection string</param>
//        public UserStore(string connectionString, Func<string, string> partitionKeyFromId)
//        {
//            //_partitionKeyFromId = partitionKeyFromId;
//            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
//            //var tableClient = storageAccount.CreateCloudTableClient();
//            //_userTableReference = tableClient.GetTableReference("Users");
//            //_userTableReference.CreateIfNotExists();

//            //_loginTableReference = tableClient.GetTableReference("Logins");
//            //_loginTableReference.CreateIfNotExists();
//        }
//        public void Dispose()
//        {
//        }

//        public async Task CreateAsync(TUser user)
//        {
//            var partitionKey = _partitionKeyFromId(user.Id);
//            user.PartitionKey = partitionKey;
//            var operation = TableOperation.Insert(user);
//            await GetUserTable().ExecuteAsync(operation);
//        }

//        public async Task UpdateAsync(TUser user)
//        {
//            var partitionKey = _partitionKeyFromId(user.Id);
//            user.PartitionKey = partitionKey;
//            await UpdateUser(user);
//        }

//        public async Task DeleteAsync(TUser user)
//        {
//            user.ETag = "*";
//            var operation = TableOperation.Delete(user);
//            await GetUserTable().ExecuteAsync(operation);
//        }

//        public async Task<TUser> FindByIdAsync(string userId)
//        {
//            var partitionKey = _partitionKeyFromId(userId);
//            var operation = TableOperation.Retrieve<TUser>(partitionKey, userId);
//            TableResult result = await GetUserTable().ExecuteAsync(operation);
//            return (TUser)result.Result;
//        }

//        public Task<TUser> FindByNameAsync(string userName)
//        {
//            return FindByIdAsync(userName);
//        }

//        public Task SetPasswordHashAsync(TUser user, string passwordHash)
//        {
//            user.PasswordHash = passwordHash;
//            return Task.FromResult(0);
//        }

//        public Task<string> GetPasswordHashAsync(TUser user)
//        {
//            return Task.FromResult(user.PasswordHash);
//        }

//        public Task<bool> HasPasswordAsync(TUser user)
//        {
//            return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
//        }

//        public async Task AddLoginAsync(TUser user, UserLoginInfo login)
//        {
//            UserLinkedAccountsRepo.Add(new Project.Models.Models.UserModels.UserLinkedAccountDto
//            {
//                LinkId = login.ProviderKey,
//                UserId = user.Id,
//                Description = 
//            });

//            user.Logins.Add(login);
//            await UpdateUser(user);

//            var operation = TableOperation.Insert(new LoginInfoEntity(login, user.Id));
//            await GetLoginTable().ExecuteAsync(operation);
//        }

//        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
//        {
//            user.Logins.Remove(login);
//            await UpdateUser(user);

//            var operation = TableOperation.Delete(new LoginInfoEntity(login, user.Id) { ETag = "*" });
//            await GetLoginTable().ExecuteAsync(operation);
//        }

//        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
//        {
//            return Task.FromResult(user.Logins);
//        }

//        public async Task<TUser> FindAsync(UserLoginInfo login)
//        {
//            var lie = new LoginInfoEntity(login, "");
//            var operation = TableOperation.Retrieve<LoginInfoEntity>(lie.PartitionKey, lie.RowKey);
//            var result = await GetLoginTable().ExecuteAsync(operation);
//            var loginInfoEntity = (LoginInfoEntity)result.Result;
//            if (loginInfoEntity == null)
//                return null;
//            return await FindByIdAsync(loginInfoEntity.UserId);
//        }

//        public async Task AddToRoleAsync(TUser user, string role)
//        {
//            if (!user.Roles.Contains(role))
//            {
//                user.Roles.Add(role);
//                await UpdateUser(user);
//            }
//        }

//        public async Task RemoveFromRoleAsync(TUser user, string role)
//        {
//            if (user.Roles.Remove(role))
//            {
//                await UpdateUser(user);
//            }
//        }

//        public Task<IList<string>> GetRolesAsync(TUser user)
//        {
//            return Task.FromResult(user.Roles);
//        }

//        public Task<bool> IsInRoleAsync(TUser user, string role)
//        {
//            return Task.FromResult(user.Roles.Contains(role));
//        }


//        private async Task UpdateUser(TUser user)
//        {
//            var operation = TableOperation.Replace(user);
//            await GetUserTable().ExecuteAsync(operation);
//        }
//    }




//    //public class CustomUserStore : IUserStore<AuthenticatedUserModel, int>, IUserPasswordStore<AuthenticatedUserModel, int>, IUserEmailStore<AuthenticatedUserModel, int>, IUserSecurityStampStore<AuthenticatedUserModel, int>
//    //{
//    //    public Task CreateAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task DeleteAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public void Dispose()
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<AuthenticatedUserModel> FindByEmailAsync(string email)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<AuthenticatedUserModel> FindByIdAsync(int userId)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<AuthenticatedUserModel> FindByNameAsync(string userName)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<string> GetEmailAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<bool> GetEmailConfirmedAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<string> GetPasswordHashAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<string> GetSecurityStampAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task<bool> HasPasswordAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task SetEmailAsync(AuthenticatedUserModel user, string email)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task SetEmailConfirmedAsync(AuthenticatedUserModel user, bool confirmed)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task SetPasswordHashAsync(AuthenticatedUserModel user, string passwordHash)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task SetSecurityStampAsync(AuthenticatedUserModel user, string stamp)
//    //    {
//    //        throw new NotImplementedException();
//    //    }

//    //    public Task UpdateAsync(AuthenticatedUserModel user)
//    //    {
//    //        throw new NotImplementedException();
//    //    }
//    //}
//}