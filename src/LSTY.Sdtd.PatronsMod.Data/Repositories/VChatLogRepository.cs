using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class VChatLogRepository : SQLiteRepository<V_ChatLog>, IVChatLogRepository
    {
        public VChatLogRepository() : base(DataManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryBySteamId")]
        public IEnumerable<V_ChatLog> QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(V_ChatLog.SteamId), steamId);
        }

        [CatchException("Error in QueryByEntityId")]
        public IEnumerable<V_ChatLog> QueryByEntityId(int entityId)
        {
            return base.QueryById(nameof(V_ChatLog.EntityId), entityId);
        }

        [CatchException("Error in QueryByDateTime")]
        public IEnumerable<V_ChatLog> QueryByDateTime(DateTime startDateTime, DateTime endDateTime, string orderBy = null)
        {
            string whereBy = "CreatedDate BETWEEN @StartDateTime AND @EndDateTime";
            return base.Query(whereBy, orderBy, new { StartDateTime = startDateTime, EndDateTime = endDateTime });
        }
    }
}
