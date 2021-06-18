using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.WebApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.IRepositories
{
    public interface IEmailAccountRepository : IRepository<T_EmailAccount>
    {
        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> IsExist(string email);
    }
}
