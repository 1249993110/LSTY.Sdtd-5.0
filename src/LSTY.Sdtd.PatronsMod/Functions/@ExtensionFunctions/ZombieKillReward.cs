using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class ZombieKillReward : FunctionBase, ISubFunction
    {
        [ConfigNode(XmlNodeType.Attribute)]
        public int TriggerRequiredCount { get; set; } = 10;

        [ConfigNode(XmlNodeType.Attribute)]
        public int RewardPoints { get; set; } = 10;

        [ConfigNode(XmlNodeType.Attribute)]
        public string RewardPointsTips { get; set; } = "[00FF00]You earn points 10 by killed zombies";

        private ConcurrentDictionary<string, int> _zombieKill;

        private Action<Entity, Entity> _entityKilledHooked;

        public ZombieKillReward()
        {
            _entityKilledHooked = (entity1, entity2) => Task.Run(() => EntityKilled(entity1, entity2));
            availableVariables.Add("{triggerRequiredCount}");
            availableVariables.Add("{rewardPoints}");
        }

        protected override string FormatCmd(string message, OnlinePlayer player)
        {
            return base.FormatCmd(message, player)
                .Replace("{triggerRequiredCount}", TriggerRequiredCount.ToString())
                .Replace("{rewardPoints}", RewardPoints.ToString());
        }

        private void PlayerDisconnected(ClientInfo clientInfo, bool shutdown)
        {
            _zombieKill.TryRemove(clientInfo.playerId, out _);
        }

        protected override void EnableFunction()
        {
            if(_zombieKill == null)
            {
                _zombieKill = new ConcurrentDictionary<string, int>();
            }

            ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected);
            ModEvents.EntityKilled.RegisterHandler(_entityKilledHooked);
        }

        protected override void DisableFunction()
        {
            ModEvents.PlayerDisconnected.UnregisterHandler(PlayerDisconnected);
            ModEvents.EntityKilled.UnregisterHandler(_entityKilledHooked);
        }

        [CatchException("Error in EntityKilled")]
        private void EntityKilled(Entity killedEntity, Entity entityPlayer)
        {
            if (killedEntity != null 
                && entityPlayer != null 
                && killedEntity is EntityEnemy
                && killedEntity.IsClientControlled() == false
                && entityPlayer.IsClientControlled())
            {
                string steamId = ConnectionManager.Instance.Clients.ForEntityId(entityPlayer.entityId)?.playerId;

                if(LiveDataContainer.OnlinePlayers.TryGetValue(steamId,out var player))
                {
                    if(_zombieKill.TryGetValue(steamId,out int count))
                    {
                        if (++count > TriggerRequiredCount)
                        {
                            IocContainer.Resolve<IPointsRepository>().IncreasePlayerPoints(steamId, RewardPoints);
                            _zombieKill[steamId] = 0;

                            ModHelper.SendMessageToPlayer(steamId, FormatCmd(RewardPointsTips, player));

                            CustomLogger.Info("Player: {0}, steamID: {1}, reward points {2} by killed {3} zombies", 
                                player.Name, steamId, RewardPoints, TriggerRequiredCount);
                        }
                        else
                        {
                            _zombieKill[steamId] = count;
                        }
                    }
                    else
                    {
                        _zombieKill.TryAdd(steamId, 1);
                    }
                }
            }
        }
    }
}
