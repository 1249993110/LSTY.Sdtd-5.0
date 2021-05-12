using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.ViewModels
{
    public class PointsSystemConfigViewModel : FunctionManageViewModel
    {
        public string SignCmd { get; set; }

        public int SignInterval { get; set; }

        public int InitialCount { get; set; }

        public int RewardCount { get; set; }

        public string QueryPointsCmd { get; set; }

        public string SignSucceedTips { get; set; }

        public string SignFailTips { get; set; }

        public string QueryPointsTips { get; set; }

        public string NeverSignInTips { get; set; }
    }

    /// <summary>
    /// The CreatedDate field is not required
    /// </summary>
    public class PointsInfoViewModel
    {
        public string SteamId { get; set; }

        public int Count { get; set; }

        public int LastSignDay { get; set; }
    }
}
