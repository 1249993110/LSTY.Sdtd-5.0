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
using System.Threading.Tasks;
using System.Xml;


namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class OnlineReward : FunctionBase, ISubFunction
    {
        [ConfigNode(XmlNodeType.Attribute)]
        public int RewardInterval 
        { 
            get => _timer.Interval; 
            set 
            { 
                _timer.Interval = value;
            } 
        }

        [ConfigNode(XmlNodeType.Attribute)]
        public int RewardPoints { get; set; } = 1;

        [ConfigNode(XmlNodeType.Attribute)]
        public string RewardPointsTips { get; set; } = "[00FF00]You earn points 1 by keep online";

        private readonly SubTimer _timer;

        public OnlineReward()
        {
            _timer = new SubTimer(Exec, 600);
            availableVariables.Add("{rewardInterval}");
            availableVariables.Add("{rewardPoints}");
        }
        protected override string FormatCmd(string message, OnlinePlayer player)
        {
            return base.FormatCmd(message, player)
                .Replace("{rewardInterval}", RewardInterval.ToString())
                .Replace("{rewardPoints}", RewardPoints.ToString());
        }

        protected override void DisableFunction()
        {
            _timer.IsEnabled = false;
            GlobalTimer.UnregisterSubTimer(_timer);
        }

        protected override void EnableFunction()
        {
            _timer.IsEnabled = true;
            GlobalTimer.RegisterSubTimer(_timer);
        }

        private void Exec()
        {
            string steamId = null;
            foreach (var item in LiveDataContainer.OnlinePlayers)
            {
                steamId = item.Key;
                IocContainer.Resolve<IPointsRepository>().IncreasePlayerPoints(steamId, RewardPoints);

                ModHelper.SendMessageToPlayer(steamId, FormatCmd(RewardPointsTips, item.Value));

                CustomLogger.Info("Player: {0}, steamID: {1}, reward points {2} by keep online {3} minute",
                                item.Value.Name, steamId, RewardPoints, RewardInterval);
            }
        }
    }
}
