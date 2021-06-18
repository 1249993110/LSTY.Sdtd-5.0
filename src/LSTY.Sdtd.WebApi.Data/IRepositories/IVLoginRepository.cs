using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.WebApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.IRepositories
{
    public interface IVLoginRepository : IRepository<V_Login>
    {
        Task<V_Login> QueryByLoginNameAsync(string loginName);
    }
}
