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
using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Internal;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class TeleportHome : FunctionBase
    {
        private static readonly IHomePositionRepository _homePositionRepository;
        private static readonly IPointsRepository _pointsRepository;
        private static readonly ITeleRecordRepository _teleRecordRepository;

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListCmd { get; set; } = "home";

        /// <summary>
        /// uint: seconds
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public int TeleInterval { get; set; } = 20;

        [ConfigNode(XmlNodeType.Attribute)]
        public string SetHomeCmdPrefix { get; set; } = "setHome";

        [ConfigNode(XmlNodeType.Attribute)]
        public int MaxCanSetCount { get; set; } = 4;

        [ConfigNode(XmlNodeType.Attribute)]
        public int PointsRequiredForSet { get; set; } = 2;

        [ConfigNode(XmlNodeType.Attribute)]
        public string DeleteHomeCmdPrefix { get; set; } = "delHome";

        [ConfigNode(XmlNodeType.Attribute)]
        public string TeleHomeCmdPrefix { get; set; } = "goHome";

        [ConfigNode(XmlNodeType.Attribute)]
        public int PointsRequiredForTele { get; set; } = 2;

        [ConfigNode(XmlNodeType.Attribute)]
        public string NoneHaveHomeTips { get; set; } = "[00FF00]You don’t have a Home yet, please enter /setHome to set a Home";

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListPreTips { get; set; } = "[00FF00]You currently have the following home:";

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListTips { get; set; } = "[00FF00][00FF00]<[FF0000]{homeName}[00FF00]> teleCmd: [FF0000]/goHome {homeName}, points required: 2, position: {position}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string OverLimitTips { get; set; } = "[00FF00]Exceeds the maximum number of settings";

        [ConfigNode(XmlNodeType.Attribute)]
        public string SetPointsNotEnoughTips { get; set; } = "[00FF00]Not enough points! Points required: 2";

        [ConfigNode(XmlNodeType.Attribute)]
        public string SetSucceedTips { get; set; } = "[00FF00]Set successfully";

        [ConfigNode(XmlNodeType.Attribute)]
        public string OverwriteOldSucceedTips { get; set; } = "[00FF00]The old home have been overwritten successfully";

        [ConfigNode(XmlNodeType.Attribute)]
        public string DeleteSucceedTips { get; set; } = "[00FF00]Delete home successfully";

        [ConfigNode(XmlNodeType.Attribute)]
        public string HomeNotFoundTips { get; set; } = "[00FF00]Home not found";

        [ConfigNode(XmlNodeType.Attribute)]
        public string CoolingTips { get; set; } = "[00FF00]Teleported cooling... Remaining: {coolingTime} seconds ";

        [ConfigNode(XmlNodeType.Attribute)]
        public string TelePointsNotEnoughTips { get; set; } = "[00FF00]Not enough points! Points required: 2";

        [ConfigNode(XmlNodeType.Attribute)]
        public string TeleSucceedTips { get; set; } = "[00FF00]Player: {playerName}, teleported to home: {homeName}";

        static TeleportHome()
        {
            _homePositionRepository = IocContainer.Resolve<IHomePositionRepository>();
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();
            _teleRecordRepository = IocContainer.Resolve<ITeleRecordRepository>();
        }

        public TeleportHome()
        {
            availableVariables.AddRange(new string[]
            {
                "{homeName}",
                "{position}",
                "{coolingTime}"
            });
        }

        private string FormatCmd(string message, OnlinePlayer player, T_HomePosition position, int? coolingTime = null)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(message, player));

            if (coolingTime.HasValue)
            {
                builder.Replace("{coolingTime}", coolingTime.Value.ToString());
            }

            return builder
                .Replace("{homeName}", position.HomeName)
                .Replace("{position}", position.Position).ToString();
        }

        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            if (string.Equals(message, QueryListCmd, StringComparison.OrdinalIgnoreCase))
            {
                string steamId = player.SteamId;

                var positions = _homePositionRepository.Query("SteamId=@SteamId", "HomeName", new { steamId });

                if (positions.Any() == false)
                {
                    ModHelper.SendMessageToPlayer(steamId, NoneHaveHomeTips);
                }
                else
                {
                    ModHelper.SendMessageToPlayer(steamId, QueryListPreTips);

                    int index = 0;
                    foreach (var item in positions)
                    {
                        ++index;
                        ModHelper.SendMessageToPlayer(steamId,
                            string.Format("[00FF00]{0}. {1}", index, FormatCmd(QueryListTips, player, item)));
                    }
                }
            }
            else if (message.StartsWith(SetHomeCmdPrefix, StringComparison.OrdinalIgnoreCase))
            {
                //string[] args = message.Split(' ');

                //if(args.Length != 2)
                //{
                //    ModHelper.SendMessageToPlayer(steamId, "Wrong number of arguments, expected 2, found " + args.Length);
                //    return false;
                //}
                string steamId = player.SteamId;

                int cmdLength = SetHomeCmdPrefix.Length + 1;
                string homeName = message.Length <= cmdLength ? string.Empty : message.Substring(cmdLength);

                int playerPoints = _pointsRepository.QueryPointsCountBySteamId(steamId);
                if (playerPoints < PointsRequiredForSet)
                {
                    ModHelper.SendMessageToPlayer(steamId, FormatCmd(SetPointsNotEnoughTips, player));
                }
                else
                {
                    var entity = _homePositionRepository.Query("SteamId=@SteamId AND HomeName=@HomeName", null,
                        new { SteamId = steamId, HomeName = homeName }).FirstOrDefault();

                    string pos = player.LastPosition.ToString();

                    // new home postion
                    if (entity == null)
                    {
                        long positionCount = _homePositionRepository.QueryRecordCountBySteamId(steamId);

                        if (positionCount >= MaxCanSetCount)
                        {
                            ModHelper.SendMessageToPlayer(steamId, FormatCmd(OverLimitTips, player));
                            return true;
                        }
                        else
                        {
                            entity = new T_HomePosition()
                            {
                                Id = Guid.NewGuid().ToString(),
                                HomeName = homeName,
                                SteamId = steamId,
                                Position = pos
                            };

                            _homePositionRepository.Insert(entity);

                            ModHelper.SendMessageToPlayer(steamId, FormatCmd(SetSucceedTips, player, entity));
                        }
                    }
                    else
                    {
                        entity.HomeName = homeName;
                        entity.SteamId = steamId;
                        entity.Position = pos;
                        _homePositionRepository.Update(entity);

                        ModHelper.SendMessageToPlayer(steamId, FormatCmd(OverwriteOldSucceedTips, player, entity));
                    }

                    _pointsRepository.DeductPlayerPoints(steamId, PointsRequiredForSet);
                    CustomLogger.Info(string.Format("Player: {0}, steamID: {1}, set home: {2}, position: {3}", player.Name, steamId, homeName, pos));
                }
            }
            else if (message.StartsWith(DeleteHomeCmdPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string steamId = player.SteamId;

                int cmdLength = DeleteHomeCmdPrefix.Length + 1;
                string homeName = message.Length <= cmdLength ? string.Empty : message.Substring(cmdLength);

                int count = _homePositionRepository.Delete("SteamId=@SteamId AND HomeName=@HomeName", new { SteamId = steamId, HomeName = homeName });
                if (count == 0)
                {
                    ModHelper.SendMessageToPlayer(steamId, HomeNotFoundTips);
                }
                else if (count == 1)
                {
                    ModHelper.SendMessageToPlayer(steamId, DeleteSucceedTips);
                }
                else
                {
                    CustomLogger.Warn(string.Format("Player: {0}, steamId: {1}, delete home: {2} failed!", player.Name, steamId, homeName));
                }
            }
            else if (message.StartsWith(TeleHomeCmdPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string steamId = player.SteamId;

                int cmdLength = TeleHomeCmdPrefix.Length + 1;
                string homeName = message.Length <= cmdLength ? string.Empty : message.Substring(cmdLength);

                var entity = _homePositionRepository.Query("SteamId=@SteamId AND HomeName=@HomeName", null,
                                new { SteamId = steamId, HomeName = homeName }).FirstOrDefault();

                if (entity == null)
                {
                    ModHelper.SendMessageToPlayer(steamId, HomeNotFoundTips);
                }
                else
                {
                    var teleRecord = _teleRecordRepository.QueryNewest(steamId, TeleTargetTypes.Home);
                    CustomLogger.Warn(teleRecord.ToJson());
                    if (teleRecord != null)
                    {
                        int timeSpan = (int)(DateTime.Now - teleRecord.CreatedDate).TotalSeconds;
                        if (timeSpan < TeleInterval)// Cooling
                        {
                            ModHelper.SendMessageToPlayer(steamId, FormatCmd(CoolingTips, player, entity, TeleInterval - timeSpan));

                            return true;
                        }
                    }

                    int pointsCount = _pointsRepository.QueryPointsCountBySteamId(steamId);
                    if (pointsCount < PointsRequiredForTele)// Points not enough
                    {
                        ModHelper.SendMessageToPlayer(steamId, FormatCmd(TelePointsNotEnoughTips, player, entity));
                    }
                    else
                    {
                        _pointsRepository.DeductPlayerPoints(steamId, PointsRequiredForTele);

                        ModHelper.TelePlayer(player.SteamId, entity.Position);

                        ModHelper.SendGlobalMessage(FormatCmd(TeleSucceedTips, player, entity));

                        // Record delivery date
                        _teleRecordRepository.Insert(new T_TeleRecord()
                        {
                            SteamId = steamId,
                            DestinationName = entity.HomeName,
                            TargetType = TeleTargetTypes.Home,
                            Position = entity.Position
                        });

                        CustomLogger.Info("Player: {0}, steamID: {1}, teleported to: {2}", player.Name, steamId, entity.HomeName);
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
