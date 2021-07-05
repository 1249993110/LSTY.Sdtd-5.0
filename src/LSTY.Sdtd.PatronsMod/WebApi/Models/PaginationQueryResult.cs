using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class PaginationQueryResult
    {
        /// <summary>
        /// 结果总条数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 结果项
        /// </summary>
        public object Items { get; set; }
    }
}
