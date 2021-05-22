using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class TeleportHomeConfigViewModel : FunctionManageViewModel
    {
        public string QueryListCmd { get; set; }
        public int TeleInterval { get; set; }
        public string SetHomeCmdPrefix { get; set; }
        public int MaxCanSetCount { get; set; }
        public int PointsRequiredForSet { get; set; }
        public string DeleteHomeCmdPrefix { get; set; }
        public string TeleHomeCmdPrefix { get; set; }
        public int PointsRequiredForTele { get; set; }
        public string NoneHaveHomeTips { get; set; }
        public string OwnedHomeTips { get; set; } 
        public string HomePositionTips { get; set; } 
        public string OverLimitTips { get; set; } 
        public string SetPointsNotEnoughTips { get; set; }
        public string SetSucceedTips { get; set; } 
        public string OverwriteOldSucceedTips { get; set; } 
        public string DeleteSucceedTips { get; set; } 
        public string HomeNotFoundTips { get; set; } 
        public string CoolingTips { get; set; }
        public string TelePointsNotEnoughTips { get; set; } 
        public string TeleSucceedTips { get; set; } 
    }

    public class HomePositionViewModelBase
    {
        public string HomeName { get; set; }
        public string SteamId { get; set; }
        public string Position { get; set; }
    }

    public class HomePositionViewModel : HomePositionViewModelBase
    {
        public string Id { get; set; }
    }
}
