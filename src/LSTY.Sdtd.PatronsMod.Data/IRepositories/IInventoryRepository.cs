using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IInventoryRepository : IRepository<T_Inventory>
    {
        T_Inventory QueryBySteamId(string steamId);
    }
}
