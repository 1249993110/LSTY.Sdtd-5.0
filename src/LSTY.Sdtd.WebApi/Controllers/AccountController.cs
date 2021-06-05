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
using System.Diagnostics;
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
        public JsonResult<ResponseModel<int>> Get(int i)
        {
            throw new Exception("Test");
            return new SucceededNullResult();

            return new ResponseModel<int>();

            //return await Task.Run(()=> 
            //{ 
            //    return new ResponseResult<object>(null); 
            //});

            //if (true)
            //{
            //    return new FailedResult(Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            //}
            //else
            //{
            //    return new SucceededNullResult();
            //}
        }
    }
}
