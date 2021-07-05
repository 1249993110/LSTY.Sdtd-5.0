using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class KillReward : FunctionBase
    {
        private readonly IPointsRepository _pointsRepository;
        private readonly IKillRewardRepository _killRewardRepository;
        private Action<Entity> _entitySpawnedHooked;
        private Action<Entity, Entity> _entityKilledHooked;

        public KillReward()
        {
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();
            _killRewardRepository = IocContainer.Resolve<IKillRewardRepository>();

            _entitySpawnedHooked = (entity) => Task.Run(() => EntitySpawned(entity));
            _entityKilledHooked = (entity1, entity2) => Task.Run(() => EntityKilled(entity1, entity2));

            availableVariables.Add("{position}");
        }

        private string FormatCmd(string message, OnlinePlayer player, string position)
        {
            return base.FormatCmd(message, player).Replace("{position}", position);
        }

        private string FormatCmd(string message, string position)
        {
            return message.Replace("{position}", position);
        }

        protected override void EnableFunction()
        {
            CustomModEvents.EntitySpawned += _entitySpawnedHooked;
            ModEvents.EntityKilled.RegisterHandler(_entityKilledHooked);
        }

        protected override void DisableFunction()
        {
            CustomModEvents.EntitySpawned -= _entitySpawnedHooked;
            ModEvents.EntityKilled.UnregisterHandler(_entityKilledHooked);
        }

        [CatchException("Error in EntitySpawned")]
        private void EntitySpawned(Entity entity)
        {
            if(entity is EntityPlayer entityPlayer)
            {
                string steamId = ConnectionManager.Instance.Clients.ForEntityId(entityPlayer.entityId)?.playerId;

                if (LiveDataContainer.OnlinePlayers.TryGetValue(steamId, out var player))
                {
                    var killReward = _killRewardRepository.QueryById(nameof(T_KillReward.SteamIdOrEntityName), steamId).FirstOrDefault();

                    if (killReward != null)
                    {
                        ModHelper.SendGlobalMessage(FormatCmd(killReward.SpawnedTips, player, player.LastPosition.ToString()));
                    }
                }
            }
            else if (entity is EntityAlive entityAlive)
            {
                var killReward = _killRewardRepository.QueryById(nameof(T_KillReward.SteamIdOrEntityName), entityAlive.EntityName).FirstOrDefault();

                if (killReward != null)
                {
                    ModHelper.SendGlobalMessage(FormatCmd(killReward.SpawnedTips, new Position(entityAlive.position).ToString()));
                }
            }
        }

        [CatchException("Error in EntityKilled")]
        private void EntityKilled(Entity killedEntity, Entity entityThatKilledMe)
        {
            if (killedEntity != null 
                && entityThatKilledMe != null
                && entityThatKilledMe is EntityPlayer entityPlayer
                && entityThatKilledMe.IsClientControlled())
            {
                string entityPlayerSteamId = ConnectionManager.Instance.Clients.ForEntityId(entityPlayer.entityId)?.playerId;
                if (LiveDataContainer.OnlinePlayers.TryGetValue(entityPlayerSteamId, out var player))
                {
                    if (killedEntity is EntityPlayer diedPlayer && killedEntity.IsClientControlled())
                    {
                        string steamId = ConnectionManager.Instance.Clients.ForEntityId(diedPlayer.entityId)?.playerId;
                        Execute(steamId, player);
                    }
                    else if (killedEntity is EntityAlive diedEntity && killedEntity.IsClientControlled() == false)
                    {
                        string entityName = diedEntity.EntityName;
                        Execute(entityName, player);
                    }
                }
            }
        }

        private void Execute(string steamIdOrEntityName, OnlinePlayer player)
        {
            var killReward = _killRewardRepository.QueryById(nameof(T_KillReward.SteamIdOrEntityName), steamIdOrEntityName).FirstOrDefault();

            if (killReward != null)
            {
                switch (killReward.RewardContentType)
                {
                    case ContentTypes.Item:
                        ModHelper.GiveItem(player.EntityId, killReward.RewardContent, killReward.RewardCount, killReward.RewardQuality);
                        break;
                    case ContentTypes.Block:
                        ModHelper.GiveItem(player.EntityId, killReward.RewardContent, killReward.RewardCount);
                        break;
                    case ContentTypes.Entity:
                        for (int i = 0; i < killReward.RewardCount; ++i)
                        {
                            ModHelper.SpawnEntity(player.EntityId, killReward.RewardContent);
                        }
                        break;
                    case ContentTypes.Command:
                        for (int i = 0; i < killReward.RewardCount; ++i)
                        {
                            SdtdConsole.Instance.ExecuteSync(FormatCmd(killReward.RewardContent, player), null);
                        }

                        break;
                    case ContentTypes.Points:
                        _pointsRepository.IncreasePlayerPoints(player.SteamId, killReward.RewardCount);
                        break;
                    default:
                        throw new Exception("Invalid reward content type");
                }

                ModHelper.SendGlobalMessage(FormatCmd(killReward.KilledTips, player));

                CustomLogger.Info("Player: {0} steamId: {1} killed {2}({3})",
                    player.Name, player.SteamId, killReward.SteamIdOrEntityName, killReward.FriendlyName);
            }
        }
    }
}
