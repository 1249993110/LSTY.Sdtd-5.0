using System;
using System.Collections.Generic;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Entities
{

    /// <summary>
    ///  T_RefreshToken    
    /// </summary>
    public class T_RefreshToken
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
        public Guid? Fk_UserId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public string JwtId { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public bool IsRevorked { get; set; }

        /// <summary>
        /// 无    
        /// </summary>
        public DateTime ExpiryDate { get; set; }

    }

}
