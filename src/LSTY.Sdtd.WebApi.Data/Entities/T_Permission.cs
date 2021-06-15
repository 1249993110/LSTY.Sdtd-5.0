using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_Permission    
    /// </summary>
    public class T_Permission : EntityBase
    {
        /// <summary>
        /// Id    
        /// </summary>
        [PrimaryKey]
        public Guid Id { get; set; }

        /// <summary>
        /// 创建日期   
        /// </summary>
        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 路由开始部分 
        /// </summary>
        [Column("[Path]")]
        public string Path { get; set; }

        /// <summary>
        /// 许可类型
        /// </summary>
        [Column("[Type]")]
        public byte Type { get; set; }

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
