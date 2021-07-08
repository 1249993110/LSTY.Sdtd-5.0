using IceCoffee.AspNetCore.Models;
using System.Collections.Generic;

namespace LSTY.Sdtd.WebApi.Models
{
    /// <summary>
    /// 分页查询结果
    /// </summary>
    public class PaginationQueryResult<T> where T : IEnumerable<T>
    {
        /// <summary>
        /// 结果总条数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public T Items { get; set; }
    }

    /// <summary>
    /// 分页查询结果
    /// </summary>
    public class PaginationQueryResult
    {
        /// <summary>
        /// 结果总条数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public object Items { get; set; }
    }
}