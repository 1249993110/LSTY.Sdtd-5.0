using IceCoffee.Common.Timers;
using LSTY.Sdtd.PatronsMod.Data;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.WebApi;
using Nancy.Hosting.Self;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod
{
    public class API : IModApi
    {
        private const string _sqliteInteropTargetPath = "7DaysToDieServer_Data/Plugins/SQLite.Interop.dll";

        private const string _nancyTargetPath = "7DaysToDieServer_Data/Managed/Nancy.dll";

        private const string _magickNativeTargetPath = "7DaysToDieServer_Data/Plugins/Magick.Native-Q8-x64.dll";

        private static NancyHost _nancyHost;

        /// <summary>
        /// Init mod
        /// </summary>
        [CatchException("Initialize mod LSTY.Sdtd.PatronsMod error")]
        public void InitMod()
        {
            Task.Run(InternalInit);
        }

        [CatchException("LSTY.Sdtd.PatronsMod internal init error", true)]
        private static void InternalInit()
        {
            CustomLogger.Info("Initializing mod LSTY.Sdtd.PatronsMod");

            if (LicenseManager.CheckPermission() == false)
            {
                return;
            }

            #region Init directory. Try create directory and copy files
            
            if (Directory.Exists("LSTY") == false)
            {
                Directory.CreateDirectory("LSTY");
            }

            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (File.Exists(_sqliteInteropTargetPath) == false)
            {
                File.Copy(modPath + "/x64/SQLite.Interop.dll", _sqliteInteropTargetPath);
            }

            if (File.Exists(_nancyTargetPath) == false)
            {
                File.Copy(modPath + "/Nancy.dll", _nancyTargetPath);
            }

            if (File.Exists(_magickNativeTargetPath) == false)
            {
                File.Copy(modPath + "/x64/Magick.Native-Q8-x64.dll", _magickNativeTargetPath);
            }


            string saveGameDir = GameUtils.GetSaveGameDir();

            string dataDir = saveGameDir + "/LSTY";

            if (Directory.Exists(dataDir) == false)
            {
                Directory.CreateDirectory(dataDir);
            }

            string databasePath = saveGameDir + "/LSTY/database.db";

            CustomLogger.Info("Initializing database");

            DataManager.InitializeDatabase(databasePath);

            #endregion

            RegisterModEventHandlers();

            CustomLogger.Info("Mod LSTY.Sdtd.PatronsMod initialized successfully");
        }

        /// <summary>
        /// Register Mod Event Handlers
        /// </summary>
        [CatchException("Register mod event handlers error", true)]
        private static void RegisterModEventHandlers()
        {
            CustomLogger.Info("Register mod event handlers");
            ModEvents.GameAwake.RegisterHandler(() => Task.Run(GameAwake));
            ModEvents.GameStartDone.RegisterHandler(() => Task.Run(GameStartDone));
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);
        }

        [CatchException("Error in GameAwake")]
        private static void GameAwake()
        {
            FunctionManager.Init();

            ConfigManager.LoadAll();

            var webConfig = FunctionManager.CommonConfig.WebConfig;
            string uri = string.Format("http://localhost:{0}", webConfig.Port);

            _nancyHost = new NancyHost(new Uri(uri), new CustomBootstrapper());
            _nancyHost.Start();

            if (webConfig.OpenInDefaultBrowser)
            {
                System.Diagnostics.Process.Start(uri);
            }
        }

        [CatchException("Error in GameStartDone")]
        private static void GameStartDone()
        {
            ModHelper.GameStartDone = true;
            GlobalTimer.Start();
        }

        [CatchException("Error in GameShutdown")]
        private static void GameShutdown()
        {
            GlobalTimer.Stop();

            _nancyHost.Stop();

            ConfigManager.DisableConfigFileWatcher();
            ConfigManager.SaveAll();
        }
    }
}
