using IceCoffee.Common.Timers;
using LSTY.Sdtd.PatronsMod.Data;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.WebApi;
using Nancy.Hosting.Self;
using System;
using System.IO;
using System.Reflection;

namespace LSTY.Sdtd.PatronsMod
{
    public class API : IModApi
    {
        private const string _sqliteInteropTargetPath = "7DaysToDieServer_Data/Plugins/SQLite.Interop.dll";

        private const string _nancyTargetPath = "7DaysToDieServer_Data/Managed/Nancy.dll";

        private static NancyHost _nancyHost;

        /// <summary>
        /// Init mod
        /// </summary>
        [CatchException("Initialize mod LSTY.Sdtd.PatronsMod error")]
        public void InitMod()
        {
            CustomLogger.Info("Initializing mod LSTY.Sdtd.PatronsMod");

            if (LicenseManager.Check() == false)
            {
                return;
            }

            InitDirectory();

            FunctionManager.Init();

            RegisterModEventHandlers();

            CustomLogger.Info("Mod LSTY.Sdtd.PatronsMod initialized successfully");
        }

        /// <summary>
        /// About to try create directory and copy files
        /// </summary>
        [CatchException("Initialize mod LSTY.Sdtd.PatronsMod error", true)]
        private static void InitDirectory()
        {
            if (Directory.Exists("LSTY") == false)
            {
                Directory.CreateDirectory("LSTY");
            }

            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (File.Exists(_sqliteInteropTargetPath) == false)
            {
                File.Copy(modPath + "\\x64\\SQLite.Interop.dll", _sqliteInteropTargetPath);
            }

            if (File.Exists(_nancyTargetPath) == false)
            {
                File.Copy(modPath + "\\Nancy.dll", _nancyTargetPath);
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
        }

        /// <summary>
        /// Register Mod Event Handlers
        /// </summary>
        [CatchException("Register mod event handlers error", true)]
        private static void RegisterModEventHandlers()
        {
            CustomLogger.Info("Register mod event handlers");

            ModEvents.GameAwake.RegisterHandler(GameAwake);
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);

        }

        [CatchException("Error in GameAwake")]
        private static void GameAwake()
        {
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
