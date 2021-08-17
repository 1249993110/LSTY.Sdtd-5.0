using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_Menu    
    /// </summary>
    public class T_Menu : EntityBase
    {
        /// <summary>
        /// Id    
        /// </summary>
        [PrimaryKey, IgnoreInsert]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建日期 
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 父菜单Id 
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 菜单名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用  
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

    }

}
