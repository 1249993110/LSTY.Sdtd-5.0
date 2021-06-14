using IceCoffee.DbCore.OptionalAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_EmailAccount    
    /// </summary>
    public class T_EmailAccount
    {
        /// <summary>
        /// 用户Id  
        /// </summary>
        [PrimaryKey]
        public Guid Fk_UserId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string Email { get; set; }

    }
}
