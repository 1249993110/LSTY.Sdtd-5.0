using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_EmailAccount    
    /// </summary>
    public class T_EmailAccount : EntityBase
    {
        /// <summary>
        /// 用户Id  
        /// </summary>
        [PrimaryKey]
        public Guid Fk_UserId { get; set; }

        /// <summary>
        /// 创建日期  
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Email  
        /// </summary>
        public string Email { get; set; }

    }
}
