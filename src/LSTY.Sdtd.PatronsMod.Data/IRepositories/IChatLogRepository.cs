using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IChatLogRepository : IRepository<T_ChatLog>
    {
        IEnumerable<T_ChatLog> QueryBySteamId(string steamId);

        IEnumerable<T_ChatLog> QueryByEntityId(int entityId);

        IEnumerable<T_ChatLog> QueryByDateTime(DateTime startDateTime, DateTime endDateTime, string orderBy = null);
    }
}
