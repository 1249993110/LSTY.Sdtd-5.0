using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IGoodsRepository : IRepository<T_Goods>
    {
        T_Goods QueryByBuyCmd(string buyCmd);
    }
}
