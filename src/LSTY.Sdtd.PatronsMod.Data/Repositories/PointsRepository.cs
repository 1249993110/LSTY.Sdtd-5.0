﻿using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class PointsRepository : SQLiteRepository<T_Points>, IPointsRepository
    {
        public PointsRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryBySteamId")]
        public T_Points QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(T_Points.SteamId), steamId).FirstOrDefault();
        }

        [CatchException("Error in QueryPointsCountBySteamId")]
        public int QueryPointsCountBySteamId(string steamId)
        {
            return base.ExecuteScalar<int>($"SELECT [Count] FROM {TableName} WHERE SteamId=@SteamId", new { SteamId = steamId });
        }

        [CatchException("Error in IncreasePlayerPoints")]
        public int IncreasePlayerPoints(string steamId, int count)
        {
            return base.Execute($"UPDATE {TableName} SET [Count]=[Count]+@Count WHERE SteamId=@SteamId", new { SteamId = steamId, Count = count });
        }

        [CatchException("Error in DeductPlayerPoints")]
        public int DeductPlayerPoints(string steamId, int count)
        {
            return base.Execute($"UPDATE {TableName} SET [Count]=[Count]-@Count WHERE SteamId=@SteamId", new { SteamId = steamId, Count = count });
        }

        [CatchException("Error in ResetLastSignDay")]
        public int ResetLastSignDay(string steamId = null)
        {
            if (steamId == null)
            {
                return base.Update("LastSignDay=0", null, null);
            }
            else
            {
                return base.Update("LastSignDay=0", "SteamId=@SteamId", new { SteamId = steamId });
            }
        }
    }
}
