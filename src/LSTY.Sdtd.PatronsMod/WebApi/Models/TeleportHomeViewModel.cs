using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class TeleportCityConfigViewModel : FunctionManageViewModel
    {
        public string QueryListCmd { get; set; }
        public int TeleInterval { get; set; }
        public string QueryListTips { get; set; }
        public string TeleSucceedTips { get; set; }
        public string PointsNotEnoughTips { get; set; }
        public string CoolingTips { get; set; }
        public string NoneCityTips { get; set; }
        public string AvailableCityTips { get; set; }
    }

    public class CityPositionViewModelBase
    {
        public string CityName { get; set; }
        public string Command { get; set; }
        public int PointsRequired { get; set; }
        public string Position { get; set; }
    }

    public class CityPositionViewModel : CityPositionViewModelBase
    {
        public string Id { get; set; }
    }
}
