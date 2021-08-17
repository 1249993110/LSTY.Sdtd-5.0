using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.WebApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.IRepositories
{
    public interface IRoleRepository : IRepository<T_Role>
    {
        /// <summary>
        /// 通过角色名查询角色Id
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<Guid> QueryIdByNameAsync(string roleName);
    }
}
