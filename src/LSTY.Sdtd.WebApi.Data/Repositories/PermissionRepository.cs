using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.Repositories
{
    public class PermissionRepository : DefaultRepository<T_Permission>, IPermissionRepository
    {
        // AND (DATALENGTH(@RoutePattern)=DATALENGTH(RouteStarts) OR SUBSTRING(@RoutePattern,DATALENGTH(RouteStarts)+1,1)='/')
        private const string _permissionValidateSQL = @"SELECT 1 FROM T_Permission WHERE Id IN(SELECT Fk_PermissionId FROM T_RolePermission WHERE Fk_RoleId=@RoleId) AND (@RoutePattern LIKE CONCAT(RouteStarts,'%')) AND ([Type]=0 OR [Type]=@Type) AND IsEnabled=1";

        public async Task<bool> CheckPermissionAsync(string roleId, byte permissionType, string routePattern)
        {
            try
            {
                var result = await base.ExecuteScalarAsync<int>(_permissionValidateSQL, new
                {
                    RoleId = roleId,
                    Type = permissionType,
                    RoutePattern = routePattern
                });

                return result == 1;
            }
            catch (Exception ex)
            {
                throw new DbCoreException("确定角色是否拥有访问指定资源的权限异常", ex);
            }
        }
    }
}
