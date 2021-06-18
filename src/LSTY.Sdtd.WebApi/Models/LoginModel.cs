using IceCoffee.AspNetCore.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        public string LoginName { get; set; }

        [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
        public string PasswordHash { get; set; }
    }
}
