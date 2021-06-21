using IceCoffee.AspNetCore.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Models.Account
{
    public class EmailRegisterModelBase
    {
        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        public string AccountName { get; set; }

        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class EmailRegisterModel : EmailRegisterModelBase
    {
        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        public string Captcha { get; set; }
    }
}
