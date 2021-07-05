using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class KillRewardRepository : SQLiteRepository<T_KillReward>, IKillRewardRepository
    {
        public KillRewardRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }
    }
}
