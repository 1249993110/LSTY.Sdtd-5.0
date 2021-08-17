using IceCoffee.DbCore.Primitives.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{

    /// <summary>
    ///  V_ActiveClient    
    /// </summary>
    public class V_ActiveClient : EntityBase
    {
        /// <summary>
        /// 无    
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public int MaxInstanceCount { get; set; }

    }

}
