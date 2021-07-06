using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class CDKeyExchangeLogRepository : SQLiteRepository<T_CDKeyExchangeLog>, ICDKeyExchangeLogRepository
    {
        public CDKeyExchangeLogRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }
    }
}
