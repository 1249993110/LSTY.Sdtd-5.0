using IceCoffee.Common;
using IceCoffee.Common.Timers;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class LotterySystem : FunctionBase
    {
        private bool _isLotterying;

        /// <summary>
        /// current participant
        /// </summary>
        private readonly List<OnlinePlayer> _currentParticipant;

        private SubTimer _timer1;

        private SubTimer _timer2;

        public string CurrentLotteryCmd { get; set; }

        [ConfigNode(XmlNodeType.Attribute)]
        public int LotteryInterval
        {
            get => _timer1.Interval;
            set
            {
                _timer1.Interval = value;
            }
        }

        [ConfigNode(XmlNodeType.Attribute)]
        public int LotteryDuration
        {
            get => _timer2.Interval;
            set
            {
                _timer2.Interval = value;
            }
        }

        [ConfigNode(XmlNodeType.Attribute)]
        public int MaxWinnerCount { get; set; } = 1;

        [ConfigNode(XmlNodeType.Attribute)]
        public string StartLotteryTips { get; set; } = "[FF6699]ヽ[FFFF00]([FF0000]●[000000]'[FFFF00]▼[000000]'[FF0000]●[FFFF00])[ff6699]ノ[80F1FF]Lottery is start! Press[ff0000]Ｔ[80F1FF] input：[ff0000]{lotteryCmd}[80F1FF], join";

        [ConfigNode(XmlNodeType.Attribute)]
        public string EndLotteryTips { get; set; } = "[00FF00]The lottery is over, the winners are: {winners}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string WinningTips { get; set; } = "[00FF00]Congratulations on your winning: {lotteryName}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string NotWinningTips { get; set; } = "[00FF00]Sorry you didn't win";

        private static readonly IPointsRepository _pointsRepository;
        private static readonly ILotteryRepository _lotteryRepository;
        static LotterySystem()
        {
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();
            _lotteryRepository = IocContainer.Resolve<ILotteryRepository>();
        }

        public LotterySystem()
        {
            _currentParticipant = new List<OnlinePlayer>();

            _timer1 = new SubTimer(OnStartLottery, 300);
            _timer2 = new SubTimer(OnEndLottery, 30);
            GlobalTimer.RegisterSubTimer(_timer1);
            GlobalTimer.RegisterSubTimer(_timer2);

            availableVariables.AddRange(new List<string>()
            {
                "{lotteryCmd}",
                "{lotteryInterval}",
                "{lotteryDuration}",
                "{lotteryName}",
                "{lotteryCount}",
                "{lotteryQuality}",
                "{contentType}",
                "{winners}"
            });
        }

        private string FormatCmd(string message, OnlinePlayer player, T_Lottery lotteryItem = null, IEnumerable<string> winnerNames = null)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(message, player));

            builder.Replace("{lotteryCmd}", FunctionManager.CommonConfig.ChatCommandPrefix + CurrentLotteryCmd)
                .Replace("{lotteryInterval}", LotteryInterval.ToString())
                .Replace("{lotteryDuration}", LotteryDuration.ToString());

            if (lotteryItem != null)
            {
                builder.Replace("{lotteryName}", lotteryItem.Name)
                .Replace("{lotteryCount}", lotteryItem.Count.ToString())
                .Replace("{lotteryQuality}", lotteryItem.Quality.ToString())
                .Replace("{contentType}", lotteryItem.ContentType);
            }

            if (winnerNames != null)
            {
                builder.Replace("{winners}", string.Join(",", winnerNames));
            }

            return builder.ToString();
        }

        protected override void DisableFunction()
        {
            _timer1.IsEnabled = false;
            base.DisableFunction();
        }

        protected override void EnableFunction()
        {
            _timer1.IsEnabled = true;
            base.EnableFunction();
        }

        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            if (_isLotterying == false)
            {
                return false;
            }
            else
            {
                if (message == CurrentLotteryCmd)
                {
                    _currentParticipant.Add(player);
                    return true;
                }

                return false;
            }
        }

        private void OnStartLottery()
        {
            if (_lotteryRepository.QueryRecordCount() < 1L)
            {
                return;
            }

            _isLotterying = true;
            _currentParticipant.Clear();
            CurrentLotteryCmd = CommonHelper.GetRandomString(3, true, true);
            base.EnableFunction();

            ModHelper.SendGlobalMessage(FormatCmd(StartLotteryTips, null));

            _timer2.IsEnabled = true;
        }

        private void OnEndLottery()
        {
            _isLotterying = false;
            _timer2.IsEnabled = false;
            base.DisableFunction();

            int maxWinnerCount = _currentParticipant.Count > MaxWinnerCount ? MaxWinnerCount : _currentParticipant.Count;

            if (maxWinnerCount <= 0)
            {
                return;
            }

            var winners = CommonHelper.Shuffle(_currentParticipant).Take(maxWinnerCount);
            var winnerNames = winners.Select(s => s.Name);
            ModHelper.SendGlobalMessage(FormatCmd(EndLotteryTips, null, null, winnerNames));

            foreach (var player in winners)
            {
                var lotteryItem = _lotteryRepository.Query(orderBy: "RANDOM() LIMIT 1").FirstOrDefault();
                if (lotteryItem == null)
                {
                    CustomLogger.Warn("No lottery, please add");
                    return;
                }

                switch (lotteryItem.ContentType)
                {
                    case ContentTypes.Item:
                        ModHelper.GiveItem(player.EntityId, lotteryItem.Content, lotteryItem.Count, lotteryItem.Quality);
                        break;
                    case ContentTypes.Block:
                        ModHelper.GiveItem(player.EntityId, lotteryItem.Content, lotteryItem.Count);
                        break;
                    case ContentTypes.Entity:
                        for (int i = 0; i < lotteryItem.Count; ++i)
                        {
                            ModHelper.SpawnEntity(player.EntityId, lotteryItem.Content);
                        }
                        break;
                    case ContentTypes.Command:
                        for (int i = 0; i < lotteryItem.Count; ++i)
                        {
                            SdtdConsole.Instance.ExecuteSync(FormatCmd(lotteryItem.Content, player, lotteryItem), null);
                        }
                        break;
                    case ContentTypes.Points:
                        _pointsRepository.IncreasePlayerPoints(player.SteamId, lotteryItem.Count);
                        break;
                    default:
                        throw new Exception("Invalid lottery type");
                }

                ModHelper.SendMessageToPlayer(player.SteamId, FormatCmd(WinningTips, player, lotteryItem, winnerNames));

                CustomLogger.Info("Player: {0} steamId: {1} win: {2} in lottery", player.Name, player.SteamId, lotteryItem.Name);
            }
        }
    }
}
