using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class AntiCheatLogRepository : SQLiteRepository<T_AntiCheatLog>, IAntiCheatLogRepository
    {
        public AntiCheatLogRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryBySteamId")]
        public IEnumerable<T_AntiCheatLog> QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(T_AntiCheatLog.SteamId), steamId);
        }
    }
}
