using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IPlayerRepository : IRepository<T_Player>
    {
        T_Player QueryBySteamId(string steamId);
    }
}
