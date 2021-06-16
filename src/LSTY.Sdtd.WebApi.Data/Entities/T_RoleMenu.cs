using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_RoleMenu    
    /// </summary>
    public class T_RoleMenu : EntityBase
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [PrimaryKey]
        public string Fk_RoleId { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        [PrimaryKey]
        public string Fk_MenuId { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

    }
}
