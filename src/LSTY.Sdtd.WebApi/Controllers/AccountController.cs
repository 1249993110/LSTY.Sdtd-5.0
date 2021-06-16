using IceCoffee.AspNetCore;
using IceCoffee.AspNetCore.Extensions;
using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Models.Results;
using IceCoffee.AspNetCore.Resources;
using IceCoffee.Common.Extensions;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    /// AccountController
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserInfo _userInfo;

        public AccountController(ILogger<AccountController> logger,
            IStringLocalizer<SharedResource> localizer,
            TokenValidationParameters tokenValidationParams,
            IRefreshTokenRepository refreshTokenRepository,
            UserInfo userInfo)
        {
            _logger = logger;
            _localizer = localizer;
            _tokenValidationParams = tokenValidationParams;
            _refreshTokenRepository = refreshTokenRepository;
            _userInfo = userInfo;
        }


        [HttpPost]
        [Route("/")]
        public SucceededNullResult Login()
        {
            return new SucceededNullResult();
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<AuthResult> Register()
        {
            // 检查使用相同电子邮箱的用户是否存在
            //var existingUser = await _userManager.FindByEmailAsync(user.Email);

            //if (existingUser != null)
            //{
            //    return BadRequest(new RegistrationResponse()
            //    {
            //        Errors = new List<string>()
            //        {
            //            "Email already in use"
            //        },
            //        Success = false
            //    });
            //}

            var jwtToken = await GenerateJwtToken(new UserInfo
            {
                UserId = "9563ebe1-9358-1b75-2b93-349747ed5713",
                RoleId = "6018c1f8-2ce9-6539-6e84-fbb7fc4d94f9",
                DisplayName = "TestUser",
                Email = "1249993110@qq.com",
                StandardAccountName = "icecoffee"
            });

            return new AuthResult()
            {
                Code = CustomStatusCode.Succeeded,
                Data = new JwtToken()
                {
                    AccessToken = jwtToken.AccessToken,
                    RefreshToken = jwtToken.RefreshToken
                }
            };
        }

       

        /// <summary>
        /// RefreshToken
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [DevelopmentResponseType(typeof(AuthResult), 200)]
        public async Task<IRespResult> RefreshToken([FromBody] JwtToken tokenRequest)
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

            return result;
        }

        private async Task<JwtToken> GenerateJwtToken(UserInfo userInfo)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Aud, _tokenValidationParams.ValidAudience),
                    new Claim(JwtRegisteredClaimNames.Iss, _tokenValidationParams.ValidIssuer),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimNames.UserId, userInfo.UserId),
                    new Claim(JwtClaimNames.RoleId, userInfo.RoleId),
                    new Claim(JwtClaimNames.DisplayName, userInfo.DisplayName),
                    new Claim(JwtClaimNames.Email, userInfo.Email),
                    new Claim(JwtClaimNames.StandardAccountName, userInfo.StandardAccountName)
                }),
                // 比较合理的值为 5~10 分钟，这里设置 30 秒只是作演示用
                Expires = DateTime.UtcNow.AddSeconds(160),
                SigningCredentials = new SigningCredentials(_tokenValidationParams.IssuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var utcNow = DateTime.UtcNow;
            var refreshToken = new T_RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevorked = false,
                Fk_UserId = userInfo.UserId,
                CreatedUtcDate = utcNow,
                ExpiryDate = utcNow.AddMonths(1),
                Value = Guid.NewGuid().ToString()
            };

            await _refreshTokenRepository.InsertAsync(refreshToken);

            return new JwtToken()
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Value
            };
        }

        private async Task<AuthResult> VerifyAndGenerateToken(JwtToken token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validation 1 - Validation JWT token format
                // 此验证功能将确保 Token 满足验证参数，并且它是一个真正的 token 而不仅仅是随机字符串
                var tokenInVerification = jwtTokenHandler.ValidateToken(token.AccessToken, _tokenValidationParams, out var validatedToken);

                // Validation 2 - Validate encryption alg
                // 检查 token 是否有有效的安全算法
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Validation 3 - validate expiry date
                // 验证原 token 的过期时间，得到 unix 时间戳
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = DateTimeExtension.FromTimeStamp(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Message = "Token has not yet expired"
                    };
                }

                // validation 4 - validate existence of the token
                // 验证 refresh token 是否存在，是否是保存在数据库的 refresh token
                var storedRefreshToken = (await _refreshTokenRepository.QueryByIdAsync("[Value]", token.RefreshToken)).FirstOrDefault();

                if (storedRefreshToken == null)
                {
                    return new AuthResult()
                    {
                        Message = "Refresh Token does not exist"
                    };
                }

                // Validation 5 - 检查存储的 RefreshToken 是否已过期
                // Check the date of the saved refresh token if it has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AuthResult()
                    {
                        Message = "Refresh Token has expired, user needs to re-login"
                    };
                }

                // Validation 6 - validate if used
                // 验证 refresh token 是否已使用
                if (storedRefreshToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Message = "Refresh Token has been used"
                    };
                }

                // Validation 7 - validate if revoked
                // 检查 refresh token 是否被撤销
                if (storedRefreshToken.IsRevorked)
                {
                    return new AuthResult()
                    {
                        Message = "Refresh Token has been revoked"
                    };
                }

                // Validation 8 - validate the id
                // 这里获得原 JWT token Id
                var jti = tokenInVerification.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Jti).Value;

                // 根据数据库中保存的 Id 验证收到的 token 的 Id
                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Message = "The token doesn't mateched the saved token"
                    };
                }

                // update current token 
                // 将该 refresh token 设置为已使用
                storedRefreshToken.IsUsed = true;
                await _refreshTokenRepository.UpdateAsync(storedRefreshToken);

                // 生成一个新的 token
                var jwtToken = await GenerateJwtToken(_userInfo);

                return new AuthResult()
                {
                    Code = CustomStatusCode.Succeeded,
                    Data = new JwtToken()
                    {
                        AccessToken = jwtToken.AccessToken,
                        RefreshToken = jwtToken.RefreshToken
                    }
                };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {
                    return new AuthResult()
                    {
                        Message = "Token has expired please re-login"
                    };
                }
                else
                {
                    return new AuthResult()
                    {
                        Message = "Something went wrong."
                    };
                }
                throw;
            }
        }
    }
}
