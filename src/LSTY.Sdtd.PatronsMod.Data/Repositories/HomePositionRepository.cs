﻿using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Data.Repositories
{
    public class HomePositionRepository : SQLiteRepository<T_HomePosition>, IHomePositionRepository
    {
        public HomePositionRepository() : base(DataManager.DefaultConnectionInfo)
        {
        }

        [CatchException("Error in QueryBySteamId")]
        public IEnumerable<T_HomePosition> QueryBySteamId(string steamId)
        {
            return base.QueryById(nameof(T_HomePosition.SteamId), steamId);
        }
    }
}
