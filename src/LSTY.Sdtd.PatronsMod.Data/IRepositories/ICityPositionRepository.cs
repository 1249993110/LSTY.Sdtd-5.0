using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.IRepositories
{
    public interface ICityPositionRepository : IRepository<T_CityPosition>
    {
        T_CityPosition QueryByCommand(string command);
    }
}
