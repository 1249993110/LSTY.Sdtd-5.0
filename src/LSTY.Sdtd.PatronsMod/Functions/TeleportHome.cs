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

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class TeleportHome : FunctionBase
    {
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
        public string TeleHomeCmdPrefix { get; set; } = "goHome";

        [ConfigNode(XmlNodeType.Attribute)]
        public int PointsRequiredForTele { get; set; } = 2;

        [ConfigNode(XmlNodeType.Attribute)]
        public string NoneHaveHomeTips { get; set; } = "[00FF00]You don’t have a Home yet, please enter /setHome to set a Home";

        [ConfigNode(XmlNodeType.Attribute)]
        public string OwnedHomeTips { get; set; } = "[00FF00]You currently have the following home:";

        [ConfigNode(XmlNodeType.Attribute)]
        public string HomePositionTips { get; set; } = "[00FF00][00FF00]<[FF0000]{homeName}[00FF00]> teleCmd：[FF0000]/goHome {homeName}, points required：{pointsRequiredForSet}, position：{position}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string OverLimitTips { get; set; } = "[00FF00]Exceeds the maximum number of settings";

        [ConfigNode(XmlNodeType.Attribute)]
        public string SetPointsNotEnoughTips { get; set; } = "[00FF00]Not enough points! Points required: {pointsRequiredForSet}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string OverwriteOldSucceedTips { get; set; } = "[00FF00]The old home have been successfully overwritten";

        [ConfigNode(XmlNodeType.Attribute)]
        public string SetSucceedTips { get; set; } = "[00FF00]Set successfully";

        [ConfigNode(XmlNodeType.Attribute)]
        public string TelePointsNotEnoughTips { get; set; } = "[00FF00]Not enough points! Points required: 2";

        [ConfigNode(XmlNodeType.Attribute)]
        public string HomeNotExistTips { get; set; } = "[00FF00]Home not exist";

        [ConfigNode(XmlNodeType.Attribute)]
        public string CoolingTips { get; set; } = "[00FF00]Teleported cooling... Remaining: {coolingTime} seconds ";

        [ConfigNode(XmlNodeType.Attribute)]
        public string TeleSucceedTips { get; set; } = "[00FF00]Player: {playerName}, teleported to own home: {homeName}";

        private static readonly IHomePositionRepository _homePositionRepository;
        private static readonly IPointsRepository _pointsRepository;
        private static readonly ITeleRecordRepository _teleRecordRepository;

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
                "{pointsRequiredForSet}",
                "{position}",
                "{coolingTime}"
            });
        }

        private string FormatCmd(OnlinePlayer player, string message, T_CityPosition cityPosition, int? coolingTime = null)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(player, message));

            if (coolingTime.HasValue)
            {
                builder.Replace("{coolingTime}", coolingTime.Value.ToString());
            }

            return builder
                .Replace("{cityName}", cityPosition.CityName)
                .Replace("{teleCmd}", cityPosition.Command)
                .Replace("{pointsRequiredForSet}", cityPosition.PointsRequired.ToString())
                .Replace("{position}", cityPosition.Position).ToString();
        }

        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            string steamId = player.SteamId;
            if (message == QueryListCmd)
            {
                var cityPositions = _cityPositionRepository.QueryAll("CityName ASC");

                if (cityPositions.Count() == 0)
                {
                    ModHelper.SendMessageToPlayer(steamId, NoneCityTips);
                }
                else
                {
                    ModHelper.SendMessageToPlayer(steamId, AvailableCityTips);

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
                var cityPosition = _cityPositionRepository.QueryById("Command", message).FirstOrDefault();
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
                            ModHelper.SendMessageToPlayer(steamId, FormatCmd(player, TeleFailTips2, cityPosition, TeleInterval - timeSpan));

                            return true;
                        }
                    }

                    int pointsCount = _pointsRepository.QueryPointsCountBySteamId(steamId);
                    if (pointsCount < cityPosition.PointsRequired)// Points not enough
                    {
                        ModHelper.SendMessageToPlayer(steamId, FormatCmd(player, TeleFailTips1, cityPosition));
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
