using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class CDKeyExchangeConfigViewModel : FunctionManageViewModel
    {
        public string InvalidKeyTips { get; set; }
        public string ExchangeSuccessfullyTips { get; set; }
    }

    public class CDKeyExchangeViewModelBase
    {
        public string Key { get; set; }
        public bool LimitUseOnceEachPlayer { get; set; }
        public int MaxExchangeCount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ItemName { get; set; }
        public string ItemContent { get; set; }
        public int ItemCount { get; set; }
        public int ItemQuality { get; set; }
        public string ContentType { get; set; }
    }

    public class CDKeyExchangeViewModel : CDKeyExchangeViewModelBase
    {
        public int Id { get; set; }
    }

    public class T_CDKeyViewModel : CDKeyExchangeViewModel
    {
        public DateTime CreatedDate { get; set; }
    }
}
