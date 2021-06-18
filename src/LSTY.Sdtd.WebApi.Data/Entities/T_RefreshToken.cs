using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_RefreshToken    
    /// </summary>
    public class T_RefreshToken : EntityBase
    {
        /// <summary>
        /// Id    
        /// </summary>
        [PrimaryKey]
        public string Id { get; set; }

        /// <summary>
        /// 创建日期，Utc 时间
        /// </summary>
        public DateTime CreatedUtcDate { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string Fk_UserId { get; set; }

        /// <summary>
        /// 使用 JwtId 映射到对应的 token  
        /// </summary>
        public string JwtId { get; set; }

        /// <summary>
        /// 是否出于安全原因已将其撤销
        /// </summary>
        public bool IsRevorked { get; set; }

        /// <summary>
        /// Refresh Token 的生命周期很长，可以长达数月
        /// </summary>
        public DateTime ExpiryDate { get; set; }

    }

}
