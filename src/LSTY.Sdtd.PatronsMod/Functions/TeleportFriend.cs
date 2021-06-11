using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.WebApi;
using System.Xml;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Internal;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class TeleportFriend : FunctionBase
    {
        [ConfigNode(XmlNodeType.Attribute)]
        public string TeleCmd { get; set; } = "tele";

        /// <summary>
        /// uint: seconds
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public int TeleInterval { get; set; } = 20;

        [ConfigNode(XmlNodeType.Attribute)]
        public int PointsRequired { get; set; } = 2;

        [ConfigNode(XmlNodeType.Attribute)]
        public string TeleSucceedTips { get; set; } = "[00FF00]Player: {playerName}, teleported to: {targetName}";

        /// <summary>
        /// Points not enough reminder
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string PointsNotEnoughTips { get; set; } = "[00FF00]Not enough points! Points required: {pointsRequired}";

        /// <summary>
        /// Cooling reminder
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string CoolingTips { get; set; } = "[00FF00]Teleported cooling... Remaining: {coolingTime} seconds ";
        
        [ConfigNode(XmlNodeType.Attribute)]
        public string TargetNotFoundTips { get; set; } = "[00FF00]Target player not found";

        [ConfigNode(XmlNodeType.Attribute)]
        public string TargetNotFriendTips { get; set; } = "[00FF00]Target not your friend";

        private static readonly IPointsRepository _pointsRepository;
        private static readonly ITeleRecordRepository _teleRecordRepository;

        static TeleportFriend()
        {
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();
            _teleRecordRepository = IocContainer.Resolve<ITeleRecordRepository>();
        }

        public TeleportFriend()
        {
            availableVariables.AddRange(new string[]
            {
                "{teleCmd}",
                "{teleInterval}",
                "{pointsRequired}",
                "{targetName}",
                "{coolingTime}"
            });
        }

        private string FormatCmd(string message, OnlinePlayer player, string targetName = null, int? coolingTime = null)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(message, player));

            if (coolingTime.HasValue)
            {
                builder.Replace("{coolingTime}", coolingTime.Value.ToString());
            }

            if (string.IsNullOrEmpty(targetName) == false)
            {
                builder.Replace("{targetName}", targetName);
            }

            return builder
                .Replace("{teleCmd}", FunctionManager.CommonConfig.ChatCommandPrefix + TeleCmd)
                .Replace("{teleInterval}", TeleInterval.ToString())
                .Replace("{pointsRequired}", PointsRequired.ToString()).ToString();
        }

        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            if (message.StartsWith(TeleCmd + " ", StringComparison.OrdinalIgnoreCase))
            {
                string steamId = player.SteamId;

                string targetName = message.Substring(TeleCmd.Length + 1);

                if (string.IsNullOrEmpty(targetName))
                {
                    ModHelper.SendMessageToPlayer(steamId, base.FormatCmd(TargetNotFoundTips, player));
                    return true;
                }

                var clientInfo = ConsoleHelper.ParseParamIdOrName(targetName);
                EntityPlayer targetEntityPlayer = null;

                if (clientInfo == null
                    || GameManager.Instance.World.Players.dict.TryGetValue(clientInfo.entityId, out targetEntityPlayer) == false)
                {
                    ModHelper.SendMessageToPlayer(steamId, FormatCmd(TargetNotFoundTips, player, targetName));
                    return true;
                }

                if (targetEntityPlayer.IsFriendsWith(GameManager.Instance.World.Players.dict[player.EntityId]) == false)
                {
                    ModHelper.SendMessageToPlayer(steamId, FormatCmd(TargetNotFriendTips, player, targetName));
                    return true;
                }

                var teleRecord = _teleRecordRepository.QueryNewest(steamId, TeleTargetTypes.Friend);
                if (teleRecord != null)
                {
                    int timeSpan = (int)(DateTime.Now - teleRecord.CreatedDate).TotalSeconds;
                    if (timeSpan < TeleInterval)// Cooling
                    {
                        ModHelper.SendMessageToPlayer(steamId, FormatCmd(CoolingTips, player, targetName, TeleInterval - timeSpan));
                        return true;
                    }
                }

                int pointsCount = _pointsRepository.QueryPointsCountBySteamId(steamId);
                if (pointsCount < PointsRequired)// Points not enough
                {
                    ModHelper.SendMessageToPlayer(steamId, FormatCmd(PointsNotEnoughTips, player, targetName));
                    return true;
                }

                _pointsRepository.DeductPlayerPoints(steamId, PointsRequired);

                ModHelper.TelePlayer(player.EntityId, targetEntityPlayer.entityId);

                ModHelper.SendGlobalMessage(FormatCmd(TeleSucceedTips, player, targetName));

                string targetSteamId = ConnectionManager.Instance.Clients.ForEntityId(targetEntityPlayer.entityId).playerId;

                // Record delivery date
                _teleRecordRepository.Insert(new T_TeleRecord()
                {
                    SteamId = steamId,
                    DestinationName = targetSteamId,
                    TargetType = TeleTargetTypes.Friend,
                    Position = new Position(targetEntityPlayer.position).ToString()
                });

                CustomLogger.Info("Player: {0}, steamID: {1}, teleported to: {2}", player.Name, steamId, targetSteamId);
                return true;
            }

            return false;
        }
    }
}
