using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class FunctionManageViewModel
    {
        /// <summary>
        /// Function is or no enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Function name
        /// </summary>
        public string FunctionName { get; set; }
    }
}
