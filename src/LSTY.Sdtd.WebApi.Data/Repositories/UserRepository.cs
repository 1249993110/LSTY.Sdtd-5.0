using IceCoffee.DbCore.ExceptionCatch;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using System;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.Repositories
{
    public class UserRepository : DefaultRepository<T_User>, IUserRepository
    {
        public async Task<int> UpdateLoginStateAsync(string userId, DateTime lastLoginTime, string lastLoginIpAddress)
        {
            try
            {
                return await base.UpdateAsync("LastLoginTime=@LastLoginTime,LastLoginIpAddress=@LastLoginIpAddress", "Id=@Id", new
                {
                    Id = userId,
                    LastLoginTime = lastLoginTime,
                    LastLoginIpAddress = lastLoginIpAddress
                });
            }
            catch (Exception ex)
            {
                throw new DbCoreException("更新用户最后登录时间及Ip异常", ex);
            }
        }
    }
}
