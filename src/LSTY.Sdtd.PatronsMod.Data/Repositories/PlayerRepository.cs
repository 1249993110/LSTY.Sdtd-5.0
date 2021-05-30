using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
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

    }
}
