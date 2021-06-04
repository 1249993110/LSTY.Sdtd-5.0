using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Resources;
using LSTY.Sdtd.WebApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Controllers
{
    /// <summary>
    /// class
    /// </summary>
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountController(ILogger<AccountController> logger,
            IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// GetModel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IceCoffee.AspNetCore.Models.ResponseResult<Model>), StatusCodes.Status200OK)]
        public IceCoffee.AspNetCore.Models.ResponseResult<Model> Get([FromQuery]Model model)
        {
            return new IceCoffee.AspNetCore.Models.ResponseResult<Model>() { Data = model };
        }

        ///// <summary>
        ///// GetModel
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("[action]")]
        //public Model Get1()
        //{
        //    return new Model();
        //}

        /// <summary>
        /// Model
        /// </summary>
        public class Model
        {
            /// <summary>
            /// Name
            /// </summary>
            [MaxLength(1, ErrorMessage = DataAnnotationsResource.MaxLengthAttribute_ValidationError)]
            [Required(ErrorMessage = DataAnnotationsResource.RequiredAttribute_ValidationError)]
            //[EmailAddress]
            public string Name { get; set; }// = "ZhangSan";
            /// <summary>
            /// Age
            /// </summary>
            public int Age { get; set; } = 18;
        }
    }
}
