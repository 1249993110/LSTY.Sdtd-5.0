using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface ITeleRecordRepository : IRepository<T_TeleRecord>
    {
        IEnumerable<T_TeleRecord> QueryBySteamId(string steamId);

        T_TeleRecord QueryNewest(string steamId, bool isHome);
    }
}
