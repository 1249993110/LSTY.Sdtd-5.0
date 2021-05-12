using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class LotteryRepository : SQLiteRepository<T_Lottery>, ILotteryRepository
    {
        public LotteryRepository() : base(DataManager.DefaultConnectionInfo)
        {
        }
    }
}
