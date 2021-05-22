using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IVAntiCheatLogRepository : IRepository<V_AntiCheatLog>
    {
        IEnumerable<V_AntiCheatLog> QueryBySteamId(string steamId);
    }
}
