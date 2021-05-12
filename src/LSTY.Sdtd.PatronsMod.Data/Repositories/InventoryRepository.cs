using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class InventoryRepository : SQLiteRepository<T_Inventory>, IInventoryRepository
    {
        public InventoryRepository() : base(DataManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryBySteamId")]
        public T_Inventory QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(T_Inventory.SteamId), steamId).FirstOrDefault();
        }
    }
}
