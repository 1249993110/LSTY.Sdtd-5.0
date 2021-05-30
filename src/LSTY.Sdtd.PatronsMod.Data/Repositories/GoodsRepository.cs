using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class GoodsRepository : SQLiteRepository<T_Goods>, IGoodsRepository
    {
        public GoodsRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryByBuyCmd")]
        public T_Goods QueryByBuyCmd(string buyCmd)
        {
            return base.QueryById(nameof(T_Goods.BuyCmd), buyCmd).FirstOrDefault();
        }
    }
}
