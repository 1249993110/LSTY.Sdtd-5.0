using IceCoffee.DbCore.ExceptionCatch;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using System;
using System.Collections.Generic;

namespace LSTY.Sdtd.WebApi.Data.Repositories
{
    public class MenuRepository : DefaultRepository<T_Menu>, IMenuRepository
    {
        public IEnumerable<T_Menu> QueryByMenuIds(IEnumerable<Guid> menuIds, bool filterEnabled)
        {
            try
            {
                string sql = string.Format("SELECT {0} FROM {1} WHERE {2} IN @Ids {3}",
                    Select_Statement, TableName, nameof(T_Menu.Id), filterEnabled ? "AND IsEnabled=@IsEnabled" : string.Empty);
                return base.Query<T_Menu>(sql, new 
                { 
                    Ids = menuIds,
                    IsEnabled = filterEnabled
                });
            }
            catch (System.Exception ex)
            {
                throw new DbCoreException("通过菜单Ids查询菜单异常", ex);
            }
        }
    }
}
