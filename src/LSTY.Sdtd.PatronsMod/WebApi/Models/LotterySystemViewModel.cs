using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class LotterySystemConfigViewModel : FunctionManageViewModel
    {
        public string CurrentLotteryCmd { get; set; }
        public int LotteryInterval { get; set; }
        public int LotteryDuration { get; set; }
        public int MaxWinnerCount { get; set; }
        public string StartLotteryTips { get; set; }
        public string EndLotteryTips { get; set; }
        public string WinningTips { get; set; }
        public string NotWinningTips { get; set; }
    }

    public class LotteryViewModelBase
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public int Count { get; set; }

        public int Quality { get; set; }

        public string ContentType { get; set; }
    }

    public class LotteryViewModel : LotteryViewModelBase
    {
        public string Id { get; set; }
    }
}
