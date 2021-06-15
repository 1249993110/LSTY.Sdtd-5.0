using IceCoffee.DbCore.Primitives.Entity;
using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Primitives.Repository;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Data.Primitives
{
    public class DefaultRepository<TEntity> : SqlServerRepository<TEntity>, IRepository<TEntity> where TEntity : EntityBase
    {
        public DefaultRepository() : base(ConnectionInfoManager.DefaultConnectionInfo)
        {
        }
    }
}