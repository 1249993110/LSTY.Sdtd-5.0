using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class DeathPenaltyConfigViewModel : FunctionManageViewModel
    {
        public int DeductPoints { get; set; }
        public string DeductPointsTips { get; set; }
    }

    public class OnlineRewardConfigViewModel : FunctionManageViewModel
    {
        public int RewardInterval { get; set; }
        public int RewardPoints { get; set; } 
        public string RewardPointsTips { get; set; } 
    }

    public class ZombieKillRewardConfigViewModel : FunctionManageViewModel
    {
        public int TriggerRequiredCount { get; set; }
        public int RewardPoints { get; set; } 
        public string RewardPointsTips { get; set; }
    }
}
