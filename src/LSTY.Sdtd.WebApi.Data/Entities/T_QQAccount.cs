using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_QQAccount    
    /// </summary>
    public class T_QQAccount : EntityBase
    {
        /// <summary>
        /// 用户Id    
        /// </summary>
        [PrimaryKey]
        public string Fk_UserId { get; set; }

        /// <summary>
        /// 创建日期  
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// QQ用户身份的标识 
        /// </summary>
        public string OpenId { get; set; }

    }

}
