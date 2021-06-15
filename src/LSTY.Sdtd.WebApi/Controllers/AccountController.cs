using IceCoffee.AspNetCore.Controllers;
using IceCoffee.AspNetCore.Extensions;
using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Resources;
using IceCoffee.Common.Extensions;
using LSTY.Sdtd.WebApi.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Controllers
{
    /// <summary>
    /// class
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : JwtAuthControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountController(ILogger<AccountController> logger,
            IStringLocalizer<SharedResource> localizer,
            IOptionsMonitor<JwtSettings> jwtConfigOptionsMonitor,
            TokenValidationParameters tokenValidationParams) : base(jwtConfigOptionsMonitor, tokenValidationParams)
        {
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult<AuthResult>> Login()
        {
            return await GenerateJwtToken(existingUser);
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult<AuthResult>> Register()
        {
            return await GenerateJwtToken(newUser);
        }

        /// <summary>
        /// RefreshToken
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult<SucceededNullResult>> RefreshToken([FromBody] JwtToken tokenRequest)
        {
            var result = await VerifyAndGenerateToken(tokenRequest);

            if (result == null)
            {
                string requestId = HttpContext.GetRequestId();
                return new FailedResult(requestId) 
                { 
                    Message = "Invalid tokens" 
                };
            }

            return new SucceededNullResult();
        }
    }
}
