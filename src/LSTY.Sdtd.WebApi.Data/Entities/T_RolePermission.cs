using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_RolePermission    
    /// </summary>
    public class T_RolePermission : EntityBase
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [PrimaryKey]
        public Guid Fk_RoleId { get; set; }

        /// <summary>
        /// 许可Id
        /// </summary>
        [PrimaryKey]
        public Guid Fk_PermissionId { get; set; }

        /// <summary>
        /// 创建日期  
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

    }
}
