using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  V_Login    
    /// </summary>
    public class V_Login : EntityBase
    {
        /// <summary>
        /// 无    
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string Email { get; set; }
    }
}
