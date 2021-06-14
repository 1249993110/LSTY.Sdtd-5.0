using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_VipInfo    
    /// </summary>
    public class T_VipInfo
    {
        /// <summary>
        /// 用户Id 
        /// </summary>
        public Guid Fk_UserId { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 使用期限
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 最大实例个数
        /// </summary>
        public int MaxInstanceCount { get; set; }

        /// <summary>
        /// 加密的强随机字符串
        /// </summary>
        public string SecretKey { get; set; }

    }

}
