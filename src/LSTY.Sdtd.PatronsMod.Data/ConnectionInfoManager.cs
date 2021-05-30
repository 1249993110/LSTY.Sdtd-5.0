using IceCoffee.DbCore;
using IceCoffee.DbCore.ExceptionCatch;
using IceCoffee.DbCore.Utils;
using System;
using System.IO;
using System.Reflection;

namespace LSTY.Sdtd.PatronsMod.Data
{
    public static class ConnectionInfoManager
    {
        private static string _databasePath;

        /// <summary>
        /// 默认数据库连接信息，权限分配及菜单管理所使用的数据库
        /// </summary>
        public static DbConnectionInfo DefaultConnectionInfo { get; private set; }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitializeDatabase(string databasePath)
        {
            try
            {
                _databasePath = databasePath;

                DefaultConnectionInfo = new DbConnectionInfo("Data Source=" + _databasePath, DatabaseType.SQLite);

                string sql = ReadSql();

                DBHelper.ExecuteSql(DefaultConnectionInfo, sql);
            }
            catch (Exception ex)
            {
                throw new DbCoreException("Initialize database error", ex);
            }
        }

        private static string ReadSql()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LSTY.Sdtd.PatronsMod.Data.patrons.sql"))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
