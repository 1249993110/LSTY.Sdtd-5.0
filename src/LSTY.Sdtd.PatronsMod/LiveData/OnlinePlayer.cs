using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using UnityEngine;

namespace LSTY.Sdtd.PatronsMod.LiveData
{
    public class OnlinePlayer
    {
        private readonly ClientInfo _clientInfo;
        private readonly DateTime _loginTime;
        private Inventory _inventory;
        private int _expToNextLevel;
        private int _intLevel;

        public string SteamId => _clientInfo.playerId;
        public int EntityId => _clientInfo.entityId;
        public string Name => _clientInfo.playerName;
        public string IP => _clientInfo.ip;
        public DateTime LoginTime => _loginTime;
        public int ExpToNextLevel => _expToNextLevel;
        public int Ping => _clientInfo.ping;
        public float CurrentLife => _clientInfo.latestPlayerData.currentLife;
        public float Level
        {
            get
            {
                float expForNextLevel =
                    (int)Math.Min(Progression.BaseExpToLevel * Mathf.Pow(Progression.ExpMultiplier, _intLevel + 1),
                        int.MaxValue);
                float fLevel = _intLevel + 1f - ExpToNextLevel / expForNextLevel;
                return fLevel;
            }
        }
        public float TotalPlayTime => _clientInfo.latestPlayerData.totalTimePlayed;
        public Position LastPosition => new Position(GameManager.Instance.World.Players.dict[_clientInfo.entityId].GetPosition());
        public int Score => _clientInfo.latestPlayerData.score;
        public int ZombieKills => _clientInfo.latestPlayerData.zombieKills;

        public int PlayerKills => _clientInfo.latestPlayerData.playerKills;

        public int Deaths => _clientInfo.latestPlayerData.deaths;

        public OnlinePlayer(ClientInfo clientInfo)
        {
            _clientInfo = clientInfo;
            _loginTime = DateTime.Now;
        }

        public bool LandProtectionActive
        {
            get
            {
                return GameManager.Instance.World.IsLandProtectionValidForPlayer(GameManager.Instance
                    .GetPersistentPlayerList().GetPlayerData(SteamId));
            }
        }

        public float LandProtectionMultiplier
        {
            get
            {
                return GameManager.Instance.World.GetLandProtectionHardnessModifierForPlayer(GameManager.Instance
                    .GetPersistentPlayerList().GetPlayerData(SteamId));
            }
        }

        public Inventory GetInventory()
        {
            return _inventory;
        }

        public void Update(PlayerDataFile pdf)
        {
            UpdateProgression(pdf);

            if (_inventory == null)
            {
                _inventory = Inventory.Create();
            }

            _inventory.Update(pdf);
        }

        private void UpdateProgression(PlayerDataFile pdf)
        {
            if (pdf.progressionData.Length <= 0)
            {
                return;
            }

            using (PooledBinaryReader pbr = MemoryPools.poolBinaryReader.AllocSync(false))
            {
                pbr.SetBaseStream(pdf.progressionData);
                long posBefore = pbr.BaseStream.Position;
                pbr.BaseStream.Position = 0;
                Progression p = Progression.Read(pbr, null);
                pbr.BaseStream.Position = posBefore;

                _expToNextLevel = p.ExpToNextLevel;
                _intLevel = p.Level;
            }
        }
    }
}
