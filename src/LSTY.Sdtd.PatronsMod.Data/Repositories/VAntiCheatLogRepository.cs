using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class VAntiCheatLogRepository : SQLiteRepository<V_AntiCheatLog>, IVAntiCheatLogRepository
    {
        public VAntiCheatLogRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryBySteamId")]
        public IEnumerable<V_AntiCheatLog> QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(V_AntiCheatLog.SteamId), steamId);
        }
    }
}
