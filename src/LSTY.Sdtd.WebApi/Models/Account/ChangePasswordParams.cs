using IceCoffee.AspNetCore.Resources;
using System.ComponentModel.DataAnnotations;

namespace LSTY.Sdtd.WebApi.Models.Account
{
    /// <summary>
    /// 更改密码参数
    /// </summary>
    public class ChangePasswordParams
    {
        /// <summary>
        /// 旧密码哈希值
        /// </summary>
        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        public string OldPasswordHash { get; set; }

        /// <summary>
        /// 新密码哈希值
        /// </summary>
        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        public string NewPasswordHash { get; set; }
    }
}