using IceCoffee.Common.Timers;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class DeathPenalty : FunctionBase, ISubFunction
    {
        [ConfigNode(XmlNodeType.Attribute)]
        public int DeductPoints { get; set; } = 1;

        [ConfigNode(XmlNodeType.Attribute)]
        public string DeductPointsTips { get; set; } = "[00FF00]You deduct points {2} by death penalty";

        [ConfigNode(XmlNodeType.Attribute)]
        public bool AllowNegative { get; set; } = true;

        private readonly Action<ClientInfo, RespawnType, Vector3i> _playerSpawnedInWorldHook;

        public DeathPenalty()
        {
            _playerSpawnedInWorldHook = new Action<ClientInfo, RespawnType, Vector3i>(PlayerSpawnedInWorld);
            availableVariables.Add("{deductPoints}");
        }

        private string FormatCmd(string message, OnlinePlayer player, int deductPoints)
        {
            return base.FormatCmd(message, player).Replace("{deductPoints}", deductPoints.ToString());
        }

        protected override void DisableFunction()
        {
            ModEvents.PlayerSpawnedInWorld.UnregisterHandler(_playerSpawnedInWorldHook);
        }

        protected override void EnableFunction()
        {
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(_playerSpawnedInWorldHook);
        }

        private void PlayerSpawnedInWorld(ClientInfo clientInfo, RespawnType respawnType, Vector3i position)
        {
            switch (respawnType)
            {
                case RespawnType.Died:
                    string steamId = clientInfo.playerId;
                    var pointsRepository = IocContainer.Resolve<IPointsRepository>();

                    var deductPoints = DeductPoints;

                    if (AllowNegative == false)
                    {
                        int count = pointsRepository.QueryPointsCountBySteamId(steamId);
                        if (count <= 0)
                        {
                            break;
                        }
                        else if(count < DeductPoints) 
                        {
                            deductPoints = count;
                        }
                    }

                    pointsRepository.DeductPlayerPoints(steamId, deductPoints);
                    var player = LiveDataContainer.OnlinePlayers[steamId];
                    ModHelper.SendMessageToPlayer(steamId, FormatCmd(DeductPointsTips, player, deductPoints));

                    CustomLogger.Info("Player: {0}, steamID: {1}, deduct points {2} by death penalty",
                                    player.Name, steamId, deductPoints);
                    break;
            }
        }
    }
}
