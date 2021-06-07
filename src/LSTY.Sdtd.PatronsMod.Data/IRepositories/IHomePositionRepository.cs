using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IHomePositionRepository : IRepository<T_HomePosition>
    {
        IEnumerable<T_HomePosition> QueryBySteamId(string steamId);
        long QueryRecordCountBySteamId(string steamId);
    }
}
