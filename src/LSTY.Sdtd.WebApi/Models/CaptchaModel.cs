using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Models
{
    public class CaptchaModel
    {
        public string Id { get; set; }

        public string ImageBase64 { get; set; }
    }
}
