using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{
    /// <summary>
    ///  T_QQAccount    
    /// </summary>
    public class T_QQAccount
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
        /// QQ用户身份的标识 
        /// </summary>
        public string OpenId { get; set; }

    }

}
