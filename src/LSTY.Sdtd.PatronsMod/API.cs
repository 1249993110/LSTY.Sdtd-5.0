using IceCoffee.Common.Timers;
using LSTY.Sdtd.PatronsMod.Data;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.WebApi;
using LSTY.Sdtd.PatronsMod.MapRendering;
using LSTY.Sdtd.PatronsMod.WebSocket;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp.Server;
using LSTY.Sdtd.PatronsMod.Internal;
using System.Diagnostics;

namespace LSTY.Sdtd.PatronsMod
{
    public class API : IModApi
    {
        private static NancyHost _nancyHost;
        private static WebSocketServer _webSocketServer;

        static API()
        {
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                JsonSerializerSettings setting = new JsonSerializerSettings() 
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",
                };

                return setting;
            });
        }

        /// <summary>
        /// Init mod
        /// </summary>
        [CatchException("Initialize mod LSTY.Sdtd.PatronsMod error")]
        public void InitMod()
        {
            ModHelper.MainThreadContext = SynchronizationContext.Current;

            Task.Run(InternalEarlyInit);

            // Initialize in advance map rendering
            MapRender.Init();
        }

        [CatchException("LSTY.Sdtd.PatronsMod internal init error", true)]
        private static void InternalEarlyInit()
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

            string modPath = ModHelper.ModPath;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (File.Exists("7DaysToDieServer_Data/Plugins/SQLite.Interop.dll") == false)
                {
                    File.Copy(modPath + "/x64/SQLite.Interop.dll", "7DaysToDieServer_Data/Plugins/SQLite.Interop.dll");
                }

                if (File.Exists("7DaysToDieServer_Data/Plugins/libSkiaSharp.dll") == false)
                {
                    File.Copy(modPath + "/x64/libSkiaSharp.dll", "7DaysToDieServer_Data/Plugins/libSkiaSharp.dll");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (File.Exists("7DaysToDieServer_Data/Plugins/libSQLite.Interop.so") == false)
                {
                    File.Copy(modPath + "/linux-x64/libSQLite.Interop.so", "7DaysToDieServer_Data/Plugins/libSQLite.Interop.so");
                }

                if (File.Exists("7DaysToDieServer_Data/Plugins/libSkiaSharp.so") == false)
                {
                    File.Copy(modPath + "/linux-x64/libSkiaSharp.so", "7DaysToDieServer_Data/Plugins/libSkiaSharp.so");
                }

                Process.Start("chmod", " +x " + modPath + "/restart.sh");
            }

            if (File.Exists("7DaysToDieServer_Data/Managed/Nancy.dll") == false)
            {
                File.Copy(modPath + "/Nancy.dll", "7DaysToDieServer_Data/Managed/Nancy.dll");
            }

            string saveGameDir = GameUtils.GetSaveGameDir();

            string dataDir = saveGameDir + "/LSTY";

            if (Directory.Exists(dataDir) == false)
            {
                Directory.CreateDirectory(dataDir);
            }

            string databasePath = saveGameDir + "/LSTY/database.db";

            CustomLogger.Info("Initializing database");

            ConnectionInfoManager.InitializeDatabase(databasePath);

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
            ModEvents.CalcChunkColorsDone.RegisterHandler(CalcChunkColorsDone);
        }

        [CatchException("Error in GameAwake")]
        private static void GameAwake()
        {
            FunctionManager.Init();

            ConfigManager.LoadAll();

            var webConfig = FunctionManager.CommonConfig.WebConfig;
            string uri = "http://localhost:" + webConfig.WebApiPort;

            _nancyHost = new NancyHost(new Uri(uri), new CustomBootstrapper());
            _nancyHost.Start();

            InitWebSocket();

            if (webConfig.OpenInDefaultBrowser)
            {
                Process.Start(uri);
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

            Logger.Main.LogCallbacks -= LogCallback;
            _webSocketServer.Stop();

            ConfigManager.DisableConfigFileWatcher();
            ConfigManager.SaveAll();

            MapRender.Shutdown();

            ModHelper.OnGameShutdown();
        }

        private static void InitWebSocket()
        {
            _webSocketServer = new WebSocketServer(System.Net.IPAddress.Any, FunctionManager.CommonConfig.WebConfig.WebSocketPort);
            _webSocketServer.Log.Output = (logData, path) =>
            {
                string message = "Error in WebSocket: " + logData.ToString();
                switch (logData.Level)
                {
                    case WebSocketSharp.LogLevel.Warn:
                        CustomLogger.Warn(message);
                        break;
                    case WebSocketSharp.LogLevel.Error:
                    case WebSocketSharp.LogLevel.Fatal:
                        CustomLogger.Error(message);
                        break;
                    default:
                        CustomLogger.Info(message);
                        break;
                }
            };
            _webSocketServer.AddWebSocketService<WebSocketSession>("/");
            _webSocketServer.Start();

            Logger.Main.LogCallbacks += LogCallback;
        }

        private static void LogCallback(string message, string trace, LogType type)
        {
            try
            {
#pragma warning disable CS0618 // 类型或成员已过时
                _webSocketServer.WebSocketServices.BroadcastAsync(message, null);
#pragma warning restore CS0618 // 类型或成员已过时
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in LogCallback");
            }
        }

        private static void CalcChunkColorsDone(Chunk chunk)
        {
            MapRender.RenderSingleChunk(chunk);
        }
    }
}
