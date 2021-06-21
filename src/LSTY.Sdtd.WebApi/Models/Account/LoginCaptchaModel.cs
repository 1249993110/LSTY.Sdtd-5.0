using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Models.Account
{
    public class LoginCaptchaModel
    {
        public int ValidSeconds { get; set; }

        public int RequestInterval { get; set; }

        public string ImageBase64 { get; set; }
    }
}
