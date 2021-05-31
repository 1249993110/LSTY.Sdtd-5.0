using Dapper;
using IceCoffee.DbCore;
using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class PlayerRepository : SQLiteRepository<T_Player>, IPlayerRepository
    {
        public PlayerRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }


        [CatchException("Error in QueryBySteamId")]
        public T_Player QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(T_Player.SteamId), steamId).FirstOrDefault();
        }

        [CatchException("Error in QueryNameDict")]
        public IDictionary<string, string> QueryNameDict()
        {
            var sql = "SELECT SteamId,Name FROM T_Player";

            IDbConnection dbConnection = null;
            try
            {
                dbConnection = DbConnectionFactory.GetConnectionFromPool(ConnectionInfoManager.DefaultConnectionInfo);

                using (var reader = dbConnection.ExecuteReader(sql))
                {
                    var dict = new Dictionary<string, string>();

                    while (reader.Read())
                    {
                        dict[reader.GetString(0)] = reader.GetString(1);
                    }

                    return dict;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dbConnection != null)
                {
                    DbConnectionFactory.CollectDbConnectionToPool(dbConnection);
                }
            }
        }
    }
}
