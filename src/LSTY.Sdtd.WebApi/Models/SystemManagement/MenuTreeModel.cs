using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Models.SystemManagement
{
    /// <summary>
    /// 系统菜单 tree
    /// </summary>
    public class MenuTreeModel : TreeNode<MenuTreeModel>
    {
        /// <summary>
        /// Id    
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 创建日期 
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 父菜单Id 
        /// </summary>
        public string ParentId { get; set; }

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
