using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class CityPositionRepository : SQLiteRepository<T_CityPosition>, ICityPositionRepository
    {
        public CityPositionRepository() : base(DataManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryByCommand")]
        public T_CityPosition QueryByCommand(string command)
        {
            return base.QueryById(nameof(T_CityPosition.Command), command).FirstOrDefault();
        }
    }
}
