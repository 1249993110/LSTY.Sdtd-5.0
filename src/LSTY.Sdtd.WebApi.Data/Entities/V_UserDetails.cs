using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  V_UserDetails    
    /// </summary>
    public class V_UserDetails : EntityBase
    {
        /// <summary>
        /// 无    
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string LastLoginIpAddress { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public int MaxInstanceCount { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string SecretKey { get; set; }

    }

}
