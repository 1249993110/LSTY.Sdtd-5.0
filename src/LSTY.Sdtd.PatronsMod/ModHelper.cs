using LSTY.Sdtd.PatronsMod.Primitives;
using System.Threading;

namespace LSTY.Sdtd.PatronsMod
{
    static class ModHelper
    {
        public static bool GameStartDone;

        public static SynchronizationContext MainThreadContext;

        #region ChatHelper

        public static void SendMessage(ClientInfo receiver, ClientInfo sender, string message)
        {
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

            receiver.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, message, senderName + " (PM)", false, null));

            CustomLogger.Info("Message to player {0} sent with sender {1}.", receiver.playerId, senderId);
        }

        /// <summary>
        /// Sends a global message to all connected clients.
        /// </summary>
        /// <param name="message"></param>
        public static void SendGlobalMessage(string message)
        {
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
        /// <param name="entityId"></param>
        /// <param name="position"></param>
        public static void TelePlayer(int entityId, string position)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("tele {0} {1}", entityId, position), null);
        }

        /// <summary>
        /// Teleport player
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="position"></param>
        public static void TelePlayer(int entityId, Position position)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("tele {0} {1}", entityId, position), null);
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
    }
}
