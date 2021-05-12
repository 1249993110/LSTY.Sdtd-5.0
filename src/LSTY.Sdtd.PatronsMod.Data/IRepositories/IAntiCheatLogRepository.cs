using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IAntiCheatLogRepository : IRepository<T_AntiCheatLog>
    {
        IEnumerable<T_AntiCheatLog> QueryBySteamId(string steamId);
    }
}
