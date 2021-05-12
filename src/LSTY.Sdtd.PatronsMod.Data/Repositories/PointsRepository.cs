using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class PointsRepository : SQLiteRepository<T_Points>, IPointsRepository
    {
        public PointsRepository() : base(DataManager.DefaultConnectionInfo)
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
        public void IncreasePlayerPoints(string steamId, int count)
        {
            base.Execute($"UPDATE {TableName} SET [Count]=[Count]+@Count WHERE SteamId=@SteamId", new { SteamId = steamId, Count = count });
        }

        [CatchException("Error in DeductPlayerPoints")]
        public void DeductPlayerPoints(string steamId, int count)
        {
            base.Execute($"UPDATE {TableName} SET [Count]=[Count]-@Count WHERE SteamId=@SteamId", new { SteamId = steamId, Count = count });
        }

        [CatchException("Error in ResetLastSignDay")]
        public void ResetLastSignDay(string steamId = null)
        {
            if (steamId == null)
            {
                base.Update("LastSignDate=0", null, null);
            }
            else
            {
                base.Update("LastSignDate=0", "SteamId=@SteamId", new { SteamId = steamId });
            }
        }
    }
}
