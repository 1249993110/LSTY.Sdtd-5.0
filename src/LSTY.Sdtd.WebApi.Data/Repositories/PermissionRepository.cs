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
        private const string _permissionValidateSQL = @"SELECT 1 FROM T_Permission WHERE Id IN(SELECT Fk_PermissionId FROM T_RolePermission WHERE Fk_RoleId=@RoleId) AND IsEnabled=1 AND ([Type]=0 OR [Type]=@Type) AND (@RoutePattern LIKE CONCAT(RouteStarts,'%') AND (DATALENGTH(@RoutePattern)=DATALENGTH(@RoutePattern) OR SUBSTRING(@RoutePattern,DATALENGTH(RouteStarts),1)))";

        public async Task<bool> CheckPermissionAsync(string roleId, byte permissionType, string routePattern)
        {
            try
            {
                var result = await base.QueryAsync<int>(_permissionValidateSQL, new
                {
                    RoleId = roleId,
                    Type = permissionType,
                    RoutePattern = routePattern
                });

                return result.FirstOrDefault() == 1;
            }
            catch (Exception ex)
            {
                throw new DbCoreException("确定角色是否拥有访问指定 path starts segments 资源的权限异常", ex);
            }
        }
    }
}
