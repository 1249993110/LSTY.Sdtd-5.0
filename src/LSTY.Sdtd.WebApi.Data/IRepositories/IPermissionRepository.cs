using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.WebApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.IRepositories
{
    public interface IPermissionRepository : IRepository<T_Permission>
    {
        /// <summary>
        /// 检查许可
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="routeStarts"></param>
        /// <param name="permissionType">要求的许可</param>
        /// <returns></returns>
        Task<bool> CheckPermissionAsync(string roleId, byte permissionType, string routeStarts);
    }
}
