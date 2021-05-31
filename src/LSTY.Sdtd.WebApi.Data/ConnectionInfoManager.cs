using IceCoffee.DbCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace LSTY.Sdtd.WebApi.Data
{
    /// <summary>
    /// 数据库连接信息管理器
    /// </summary>
    public static class ConnectionInfoManager
    {
        private static Dictionary<string, DbConnectionInfo> _connNameInfoCache = new Dictionary<string, DbConnectionInfo>();

        /// <summary>
        /// 默认数据库连接信息，权限分配及菜单管理所使用的数据库
        /// </summary>
        public static DbConnectionInfo DefaultConnectionInfo => _connNameInfoCache["DefaultConnection"];

        /// <summary>
        /// 数据库连接串名和数据库连接信息缓存
        /// </summary>
        public static Dictionary<string, DbConnectionInfo> ConnNameInfoCache => _connNameInfoCache;

        /// <summary>
        /// 将数据库连接信息添加到内存缓存
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="databaseType"></param>
        /// <param name="connKeyNames"></param>
        public static void AddDbConnectionInfoToCache(IConfiguration configuration, DatabaseType databaseType, IEnumerable<string> connKeyNames)
        {
            try
            {
                foreach (var item in connKeyNames)
                {
                    _connNameInfoCache.Add(item, new DbConnectionInfo(configuration.GetConnectionString(item), databaseType));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("添加数据库连接信息到缓存异常", ex);
            }
        }
    }
}
