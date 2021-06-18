using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.WebApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.IRepositories
{
    public interface IStandardAccountRepository : IRepository<T_StandardAccount>
    {
        /// <summary>
        /// 检查标准账户名是否存在
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        Task<bool> IsExist(string accountName);
    }
}
