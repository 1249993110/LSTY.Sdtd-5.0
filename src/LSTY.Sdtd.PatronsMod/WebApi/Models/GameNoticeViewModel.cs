using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class GameNoticeConfigViewModel : FunctionManageViewModel
    {
        public string WelcomeNotice { get; set; }
        public string AlternateNotice { get; set; }
        public string AlternateNotice1 { get; set; }
        public string AlternateNotice2 { get; set; }
        public string AlternateNotice3 { get; set; }
        public string AlternateNotice4 { get; set; }
        public int AlternateInterval { get; set; }
    }
}
