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
        public Guid Id { get; set; }

        /// <summary>
        /// 创建日期  
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid Fk_UserId { get; set; }

        /// <summary>
        /// Refresh Token 
        /// </summary>
        [Column("[Value]")]
        public string Value { get; set; }

        /// <summary>
        /// 使用 JwtId 映射到对应的 token  
        /// </summary>
        public string JwtId { get; set; }

        /// <summary>
        /// 如果已经使用过它，我们不想使用相同的 refresh token 生成新的 JWT token
        /// </summary>
        public bool IsUsed { get; set; }

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
