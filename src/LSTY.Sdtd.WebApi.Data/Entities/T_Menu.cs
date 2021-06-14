using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{

    /// <summary>
    ///  T_Menu    
    /// </summary>
    public class T_Menu
    {
        /// <summary>
        /// 无    
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建日期 
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 父菜单Id 
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 菜单名 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Url 
        /// </summary>
        public string Url { get; set; }

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
