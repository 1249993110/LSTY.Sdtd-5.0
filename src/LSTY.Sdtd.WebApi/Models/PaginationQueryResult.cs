using IceCoffee.AspNetCore.Models.Results;

namespace LSTY.Sdtd.WebApi.Models
{
    /// <summary>
    /// 分页查询结果
    /// </summary>
    public class PaginationQueryResult<T> : RespResult<T>
    {
        /// <summary>
        /// 结果总条数
        /// </summary>
        public long Total { get; set; }
    }
}