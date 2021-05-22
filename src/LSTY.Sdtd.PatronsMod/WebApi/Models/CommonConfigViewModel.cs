using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class CommonConfigViewModel
    {
        /// <summary>
        /// Function name
        /// </summary>
        public string FunctionName { get; set; }

        public WebConfig WebConfig { get; set; }

        public string ServerName { get; set; }

        public string ChatCommandPrefix { get; set; }

        public string HandleChatMessageError { get; set; }

        public int ChatCommandCacheMaxCount { get; set; }
    }
}
