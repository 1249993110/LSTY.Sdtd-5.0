using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IVChatLogRepository : IRepository<V_ChatLog>
    {
        IEnumerable<V_ChatLog> QueryBySteamId(string steamId);

        IEnumerable<V_ChatLog> QueryByEntityId(int entityId);

        IEnumerable<V_ChatLog> QueryByDateTime(DateTime startDateTime, DateTime endDateTime, string orderBy = null);
    }
}
