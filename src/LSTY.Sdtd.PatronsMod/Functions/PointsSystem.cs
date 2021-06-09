using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class PointsSystem : FunctionBase
    {
        /// <summary>
        /// Sign command
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string SignCmd { get; set; } = "qd";

        /// <summary>
        /// Sign interval
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public int SignInterval { get; set; } = 1;

        /// <summary>
        /// Initial points
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public int InitialCount { get; set; } = 10;

        /// <summary>
        /// Reward points
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public int RewardCount { get; set; } = 5;

        /// <summary>
        /// Query points command
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryPointsCmd { get; set; } = "cx";

        /// <summary>
        /// Sign succeed tips
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string SignSucceedTips { get; set; } = "[00FF00]Sign in successfully! You currently have points: {ownPoints}";

        /// <summary>
        /// Sign fail tips
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string SignFailTips { get; set; } = "[00FF00]You have signed in";

        /// <summary>
        /// Query points tips
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryPointsTips { get; set; } = "[00FF00]You currently have points: {ownPoints}";

        /// <summary>
        /// Never signed in tips
        /// </summary>
        [ConfigNode(XmlNodeType.Attribute)]
        public string NeverSignInTips { get; set; } = "[00FF00]You never signed in";


        private static readonly IPointsRepository _pointsRepository;

        static PointsSystem()
        {
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();

        }

        private void InitPlayerPoints(string steamId)
        {
            long count = _pointsRepository.QueryRecordCount("SteamId=@SteamId", new { SteamId = steamId });

            if(count == 0L)
            {
                var points = new T_Points()
                {
                    SteamId = steamId,
                    Count = InitialCount,
                    LastSignDay = 0
                };

                _pointsRepository.Insert(points);
            }
        }

        public PointsSystem()
        {
            LiveDataContainer.PlayerEnterFirstTime += InitPlayerPoints;

            availableVariables.AddRange(new string[]
            {
                "{signCmd}",
                "{initialCount}",
                "{rewardPoints}",
                "{queryPointsCmd}",
                "{ownPoints}"
            });
        }

        protected override void DisableFunction()
        {
            base.DisableFunction();
        }

        protected override void EnableFunction()
        {
            base.EnableFunction();
        }

        private string FormatCmd(string message, OnlinePlayer player, int ownPoints)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(message, player));

            return builder
                .Replace("{signCmd}", SignCmd)
                .Replace("{initialCount}", InitialCount.ToString())
                .Replace("{rewardPoints}", RewardCount.ToString())
                .Replace("{queryPointsCmd}", QueryPointsCmd)
                .Replace("{ownPoints}", ownPoints.ToString()).ToString();
        }

        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            string steamId = player.SteamId;
            if (string.Equals(message, SignCmd, StringComparison.OrdinalIgnoreCase))
            {
                int currentDay = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
                int lastSignDay = 0;
                var points = _pointsRepository.QueryBySteamId(steamId);

                if (points == null)// If there is no record 
                {
                    points = new T_Points()
                    {
                        SteamId = steamId,
                        Count = InitialCount + RewardCount,
                        LastSignDay = currentDay
                    };

                    _pointsRepository.Insert(points);
                }
                else
                {
                    if (points.LastSignDay != 0 && currentDay - points.LastSignDay < SignInterval)// If player have signed
                    {
                        ModHelper.SendMessageToPlayer(steamId, this.FormatCmd(SignFailTips, player, points.Count));
                        return true;
                    }
                    else//  If player have not signed in today 
                    {
                        lastSignDay = points.LastSignDay;

                        points.Count += RewardCount;
                        points.LastSignDay = currentDay;

                        _pointsRepository.Update(points);
                    }
                }

                ModHelper.SendMessageToPlayer(steamId, this.FormatCmd(SignSucceedTips, player, points.Count));

                CustomLogger.Info(string.Format("Player sign in, steamId: {0}, current day: {1}, last sign in day: {2}", steamId, currentDay, lastSignDay));
            }
            else if (string.Equals(message, QueryPointsCmd, StringComparison.OrdinalIgnoreCase))
            {
                var points = _pointsRepository.QueryBySteamId(steamId);

                if(points == null)
                {
                    ModHelper.SendMessageToPlayer(steamId, NeverSignInTips); 
                }
                else
                {
                    ModHelper.SendMessageToPlayer(steamId, this.FormatCmd(QueryPointsTips, player, points.Count));
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
