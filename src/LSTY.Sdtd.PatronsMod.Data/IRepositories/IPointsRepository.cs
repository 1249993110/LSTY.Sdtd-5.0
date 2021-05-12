using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface IPointsRepository : IRepository<T_Points>
    {
        T_Points QueryBySteamId(string steamId);

        int QueryPointsCountBySteamId(string steamId);

        void IncreasePlayerPoints(string steamId, int count);

        void DeductPlayerPoints(string steamId, int count);

        void ResetLastSignDay(string steamId = null);
    }
}
