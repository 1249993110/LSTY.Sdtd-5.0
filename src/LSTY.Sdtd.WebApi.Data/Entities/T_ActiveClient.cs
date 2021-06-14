using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{

    /// <summary>
    ///  T_ActiveClient    
    /// </summary>
    public class T_ActiveClient
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? Fk_UserId { get; set; }

        /// <summary>
        /// 客户设备Id，由mac地址和进程Id生成
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 客户端版本
        /// </summary>
        public string Version { get; set; }

    }

}
