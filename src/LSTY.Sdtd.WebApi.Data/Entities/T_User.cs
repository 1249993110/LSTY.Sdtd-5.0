using IceCoffee.DbCore.OptionalAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_User    
    /// </summary>
    public class T_User
    {
        /// <summary>
        /// 主键   
        /// </summary>
        [PrimaryKey]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建日期   
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 显示名称    
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 上次登录Ip
        /// </summary>
        public string LastLoginIpAddress { get; set; }

    }
}
