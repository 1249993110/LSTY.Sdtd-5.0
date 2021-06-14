using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_RoleMenu    
    /// </summary>
    public class T_RoleMenu
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid Fk_RoleId { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        public Guid Fk_MenuId { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

    }
}
