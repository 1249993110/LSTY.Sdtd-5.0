using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.LiveData
{
    static class LiveDataContainer
    {
        private static readonly IPlayerRepository _playerRepository;
        private static readonly IInventoryRepository _inventoryRepository;
        public static readonly Dictionary<string, OnlinePlayer> OnlinePlayers;

        /// <summary>
        /// The player enters the game for the first time 
        /// </summary>
        public static event Action<string> PlayerEnterFirstTime;

        /// <summary>
        /// Server none player
        /// </summary>
        public static event Action ServerNonePlayer;

        /// <summary>
        /// Server have player again
        /// </summary>
        public static event Action ServerHavePlayerAgain;

        static LiveDataContainer()
        {
            _playerRepository = IocContainer.Resolve<IPlayerRepository>();
            _inventoryRepository = IocContainer.Resolve<IInventoryRepository>();
            OnlinePlayers = new Dictionary<string, OnlinePlayer>();

            ModEvents.PlayerLogin.RegisterHandler(PlayerLogin);
            ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);
            ModEvents.SavePlayerData.RegisterHandler((_1, _2) => Task.Run(() => SavePlayerData(_1, _2)));
            ModEvents.PlayerSpawning.RegisterHandler(PlayerSpawning);
        }

        [CatchException("Error in PlayerLogin")]
        private static bool PlayerLogin(ClientInfo clientInfo, string message, StringBuilder stringBuilder)
        {
            return true;
        }

        [CatchException("Error in PlayerDisconnected")]
        private static void PlayerDisconnected(ClientInfo clientInfo, bool shutdown)
        {
            OnlinePlayers.Remove(clientInfo.playerId);

            if (OnlinePlayers.Count == 0)
            {
                ServerNonePlayer?.Invoke();
            }
        }

        [CatchException("Error in SavePlayerData")]
        private static void SavePlayerData(ClientInfo clientInfo, PlayerDataFile pdf)
        {
            string steamId = clientInfo.playerId;
            
            var onlinePlayer = OnlinePlayers[steamId];

            onlinePlayer.Update(pdf);

            var lastPos = onlinePlayer.LastPosition;
            T_Player player = new T_Player()
            {
                SteamId = steamId,
                EntityId = onlinePlayer.EntityId,
                IP = onlinePlayer.IP,
                Level = onlinePlayer.Level,
                Name = onlinePlayer.Name,
                LastOnline = DateTime.Now,
                LastPositionX = lastPos.X,
                LastPositionY = lastPos.Y,
                LastPositionZ = lastPos.Z,
                TotalPlayTime = onlinePlayer.TotalPlayTime,
                Deaths = onlinePlayer.Deaths,
                PlayerKills = onlinePlayer.PlayerKills,
                Score = onlinePlayer.Score,
                ZombieKills = onlinePlayer.ZombieKills
            };

            T_Inventory inventory = new T_Inventory()
            {
                SteamId = steamId,
                Content = JsonConvert.SerializeObject(onlinePlayer.GetInventory())
            };

            _playerRepository.ReplaceInto(player);

            _inventoryRepository.ReplaceInto(inventory);
        }

        [CatchException("Error in PlayerSpawning")]
        private static void PlayerSpawning(ClientInfo clientInfo, int chunkViewDim, PlayerProfile playerProfile)
        {
            string steamId = clientInfo.playerId;

            var onlinePlayer = new OnlinePlayer(clientInfo);

            if (OnlinePlayers.ContainsKey(steamId))
            {
                CustomLogger.Warn("Player already exists in online players list and will be removed, steamId: " + steamId);
                OnlinePlayers.Remove(steamId);
            }

            OnlinePlayers.Add(steamId, onlinePlayer);

            long count = _playerRepository.QueryRecordCount("SteamId=@SteamId", new { SteamId = steamId });
            if (count == 0L)
            {
                PlayerEnterFirstTime?.Invoke(steamId);
            }

            if (OnlinePlayers.Count == 1)
            {
                ServerHavePlayerAgain?.Invoke();
            }
        }
    }
}