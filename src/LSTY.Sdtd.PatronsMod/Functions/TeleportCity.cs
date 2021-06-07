﻿using LSTY.Sdtd.PatronsMod.Primitives;
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

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class TeleportCity : FunctionBase
    {
        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListCmd { get; set; } = "hc";

        /// <summary>
        /// uint: seconds
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public int TeleInterval { get; set; } = 20;

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListPreTips { get; set; } = "[00FF00]Available public cities:";

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListTips { get; set; } = "[00FF00]<[FF0000]{cityName}[00FF00]> teleCmd: [FF0000]{teleCmd}, pointsRequired: {pointsRequired}, position: {position}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string TeleSucceedTips { get; set; } = "[00FF00]Player: {playerName}, teleported to: {cityName}";

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

        /// <summary>
        /// None city reminder
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string NoneCityTips { get; set; } = "[00FF00]No public city information";

        private static readonly ICityPositionRepository _cityPositionRepository;
        private static readonly IPointsRepository _pointsRepository;
        private static readonly ITeleRecordRepository _teleRecordRepository;

        static TeleportCity()
        {
            _cityPositionRepository = IocContainer.Resolve<ICityPositionRepository>();
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();
            _teleRecordRepository = IocContainer.Resolve<ITeleRecordRepository>();
        }

        public TeleportCity()
        {
            availableVariables.AddRange(new string[]
            {
                "{cityName}",
                "{teleCmd}",
                "{pointsRequired}",
                "{position}",
                "{coolingTime}"
            });
        }

        private string FormatCmd(OnlinePlayer player, string message, T_CityPosition position, int? coolingTime = null)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(player, message));

            if (coolingTime.HasValue)
            {
                builder.Replace("{coolingTime}", coolingTime.Value.ToString());
            }

            return builder
                .Replace("{cityName}", position.CityName)
                .Replace("{teleCmd}", position.Command)
                .Replace("{pointsRequired}", position.PointsRequired.ToString())
                .Replace("{position}", position.Position).ToString();
        }

        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            string steamId = player.SteamId;
            if (string.Equals(message, QueryListCmd, StringComparison.OrdinalIgnoreCase))
            {
                var cityPositions = _cityPositionRepository.QueryAll("CityName ASC");

                if (cityPositions.Count() == 0)
                {
                    ModHelper.SendMessageToPlayer(steamId, NoneCityTips);
                }
                else
                {
                    ModHelper.SendMessageToPlayer(steamId, QueryListPreTips);

                    int index = 0;
                    foreach (var item in cityPositions)
                    {
                        ++index;
                        ModHelper.SendMessageToPlayer(steamId, 
                            string.Format("[00FF00]{0}. {1}", index, FormatCmd(player, QueryListTips, item)));
                    }
                }
            }
            else
            {
                var cityPosition = _cityPositionRepository.Query("Command=@Command COLLATE NOCASE", "CityName", new { Command = message }).FirstOrDefault();
                if (cityPosition == null)
                {
                    return false;
                }
                else
                {
                    var teleRecord = _teleRecordRepository.QueryNewest(steamId, false);

                    if (teleRecord != null)
                    {
                        int timeSpan = (int)(DateTime.Now - teleRecord.CreatedDate).TotalSeconds;
                        if (timeSpan < TeleInterval)// Cooling
                        {
                            ModHelper.SendMessageToPlayer(steamId, FormatCmd(player, CoolingTips, cityPosition, TeleInterval - timeSpan));

                            return true;
                        }
                    }

                    int pointsCount = _pointsRepository.QueryPointsCountBySteamId(steamId);
                    if (pointsCount < cityPosition.PointsRequired)// Points not enough
                    {
                        ModHelper.SendMessageToPlayer(steamId, FormatCmd(player, PointsNotEnoughTips, cityPosition));
                    }
                    else
                    {
                        _pointsRepository.DeductPlayerPoints(steamId, cityPosition.PointsRequired);

                        ModHelper.TelePlayer(player.EntityId, cityPosition.Position);

                        ModHelper.SendGlobalMessage(FormatCmd(player, TeleSucceedTips, cityPosition));

                        // Record delivery date
                        _teleRecordRepository.Insert(new T_TeleRecord() 
                        { 
                            SteamId = steamId,
                            DestinationName = cityPosition.CityName,
                            IsHome = false,
                            Position = cityPosition.Position
                        });

                        CustomLogger.Info(string.Format("Player: {0}, steamID: {1}, teleported to: {2}", player.Name, steamId, cityPosition.CityName));
                    }
                }
            }

            return true;
        }
    }
}
