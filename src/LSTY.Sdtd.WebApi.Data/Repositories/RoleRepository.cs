using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using System.Threading.Tasks;
using System.Linq;
using System;
using IceCoffee.DbCore.ExceptionCatch;

namespace LSTY.Sdtd.WebApi.Data.Repositories
{
    public class RoleRepository : DefaultRepository<T_Role>, IRoleRepository
    {
        public async Task<string> QueryIdByNameAsync(string roleName)
        {
            try
            {
                var result = await base.QueryAsync<string>("SELECT Id FROM T_Role WHERE Name=@RoleName", new
                {
                    RoleName = roleName
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new DbCoreException("通过角色名查询角色Id异常", ex);
            }
        }
    }
}
