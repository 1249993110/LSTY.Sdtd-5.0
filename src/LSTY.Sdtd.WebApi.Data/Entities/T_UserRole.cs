using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_UserRole    
    /// </summary>
    public class T_UserRole
    {
        /// <summary>
        /// 用户Id 
        /// </summary>
        public Guid Fk_UserId { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid Fk_RoleId { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

    }
}
