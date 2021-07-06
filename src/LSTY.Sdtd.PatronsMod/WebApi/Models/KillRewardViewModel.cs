using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class KillRewardConfigViewModel : FunctionManageViewModel
    {
    }

    public class KillRewardViewModelBase
    {
        public string SteamIdOrEntityName { get; set; }

        public string FriendlyName { get; set; }

        public string RewardContent { get; set; }

        public int RewardCount { get; set; }

        public int RewardQuality { get; set; }

        public string ContentType { get; set; }

        public string SpawnedTips { get; set; }

        public string KilledTips { get; set; }
    }

    public class KillRewardViewModel : KillRewardViewModelBase
    {
        public string Id { get; set; }
    }
}
