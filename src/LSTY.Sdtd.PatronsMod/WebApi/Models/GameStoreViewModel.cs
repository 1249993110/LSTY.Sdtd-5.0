using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class GameStoreConfigViewModel : FunctionManageViewModel
    {
        public string QueryListCmd { get; set; }
        public string QueryListPreTips { get; set; }
        public string QueryListTips { get; set; }
        public string BuySuccessfullyTips { get; set; }
        public string PointsNotEnoughTips { get; set; }
        public string GoodsNoFoundTips { get; set; }
    }

    public class GoodsViewModelBase
    {
        public string Name { get; set; }

        public string BuyCmd { get; set; }

        public string Content { get; set; }

        public int Count { get; set; }

        public int Quality { get; set; }

        public int Price { get; set; }

        public string ContentType { get; set; }
    }

    public class GoodsViewModel : GoodsViewModelBase
    {
        public string Id { get; set; }
    }
}
