using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Collections.Generic;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class TeleRecordRepository : SQLiteRepository<T_TeleRecord>, ITeleRecordRepository
    {
        public TeleRecordRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryBySteamId")]
        public IEnumerable<T_TeleRecord> QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(T_TeleRecord.SteamId), steamId);
        }

        [CatchException("Error in QueryNewest")]
        public T_TeleRecord QueryNewest(string steamId, bool isHome)
        {
            return base.Query<T_TeleRecord>($"SELECT * FROM {TableName} WHERE SteamId=@SteamId AND IsHome=@IsHome ORDER BY CreatedDate DESC LIMIT 1", 
                new { SteamId = steamId, IsHome = isHome }).FirstOrDefault();
        }
    }
}
