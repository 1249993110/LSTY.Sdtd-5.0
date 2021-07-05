using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace LSTY.Sdtd.PatronsMod.Internal
{
    static class ModHelper
    {
        private static bool _isRestarting;

        public static bool GameStartDone;
        public static readonly string ModPath;
        public static SynchronizationContext MainThreadContext;
        
        static ModHelper()
        {
            ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static void RestartServer(bool force = false)
        {
            _isRestarting = true;

            if (force)
            {
                InternalRestartServer();
            }
            else
            {
                SdtdConsole.Instance.ExecuteSync("shutdown", null);
            }
        }

        private static void InternalRestartServer()
        {
            string scriptPath = null;

            string serverPath = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                scriptPath = "/restart.bat";
                serverPath = AppDomain.CurrentDomain.BaseDirectory + "/startdedicated.bat";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                scriptPath = "/restart.sh";
                serverPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            Process.Start(ModHelper.ModPath + scriptPath,
                string.Format("{0} \"{1}\"", Process.GetCurrentProcess().Id, serverPath));
        }

        public static void OnGameShutdown()
        {
            if (_isRestarting)
            {
                InternalRestartServer();
            }
        }

        #region ChatHelper

        public static void SendMessage(ClientInfo receiver, ClientInfo sender, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            string senderId;
            string senderName;

            if (sender == null)
            {
                string serverName = FunctionManager.CommonConfig.ServerName;
                senderId = serverName;
                senderName = serverName;
            }
            else
            {
                senderId = sender.playerId;
                senderName = sender.playerName;
            }

            receiver.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, message, senderName, false, null));

            CustomLogger.Info("Message to player {0} sent with sender {1}.", receiver.playerId, senderId);
        }

        /// <summary>
        /// Sends a global message to all connected clients.
        /// </summary>
        /// <param name="message"></param>
        public static void SendGlobalMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, message, FunctionManager.CommonConfig.ServerName, false, null);
        }

        /// <summary>
        /// Sends a whisper message to single connected client.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="message"></param>
        public static void SendMessageToPlayer(string steamId, string message)
        {
            ClientInfo clientInfo = ConnectionManager.Instance.Clients.ForPlayerId(steamId);

            if (clientInfo == null)
            {
                CustomLogger.Info("SteamId {0} not found.", steamId);
            }
            else
            {
                SendMessage(clientInfo, null, message);
            }
        }

        /// <summary>
        /// Sends a whisper message to single connected client.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="message"></param>
        public static void SendMessageToPlayer(int entityId, string message)
        {
            ClientInfo clientInfo = ConnectionManager.Instance.Clients.ForEntityId(entityId);

            if (clientInfo == null)
            {
                CustomLogger.Info("EntityId {0} not found.", entityId);
            }
            else
            {
                SendMessage(clientInfo, null, message);
            }
        }

        #endregion

        #region Teleport player
        /// <summary>
        /// Teleport player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        public static void TelePlayer(object player, object target)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("tele {0} {1}", player, target), null);
        }

        #endregion

        #region Give Item
        public static void GiveItem(string steamId, string itemName, int count, int quality = 0, int durability = 0)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("ty-gi {0} {1} {2} {3} {4}", steamId, itemName, count, quality, durability), null);
        }
        public static void GiveItem(int entityId, string itemName, int count, int quality = 0, int durability = 0)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("ty-gi {0} {1} {2} {3} {4}", entityId, itemName, count, quality, durability), null);
        }
        #endregion

        public static void SpawnEntity(int playerEntityId, string spawnEntity)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("se {0} {1}", playerEntityId, spawnEntity), null);
        }

       

    }
}
