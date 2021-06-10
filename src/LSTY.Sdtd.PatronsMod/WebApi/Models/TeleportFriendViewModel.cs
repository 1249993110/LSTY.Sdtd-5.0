using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class TeleportFriendConfigViewModel : FunctionManageViewModel
    {
        public string TeleCmd { get; set; } 
        public int TeleInterval { get; set; } 
        public int PointsRequired { get; set; }
        public string TeleSucceedTips { get; set; }
        public string PointsNotEnoughTips { get; set; } 
        public string CoolingTips { get; set; } 
        public string TargetNotFoundTips { get; set; } 
        public string TargetNotFriendTips { get; set; }
    }
}
