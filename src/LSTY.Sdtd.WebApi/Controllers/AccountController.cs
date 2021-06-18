using IceCoffee.AspNetCore;
using IceCoffee.AspNetCore.Authorization;
using IceCoffee.AspNetCore.Extensions;
using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Models.Results;
using IceCoffee.AspNetCore.Resources;
using IceCoffee.Common;
using IceCoffee.Common.Extensions;
using IceCoffee.Common.Security.Cryptography;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using LSTY.Sdtd.WebApi.Models;
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
        private readonly IStringLocalizer<AccountResource> _localizer;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IVLoginRepository _vLoginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStandardAccountRepository _standardAccountRepository;
        private readonly IEmailAccountRepository _emailAccountRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        public AccountController(ILogger<AccountController> logger,
            IStringLocalizer<AccountResource> localizer,
            TokenValidationParameters tokenValidationParams,
            IRefreshTokenRepository refreshTokenRepository,
            IVLoginRepository vLoginRepository,
            IUserRepository userRepository,
            IStandardAccountRepository standardAccountRepository,
            IEmailAccountRepository emailAccountRepository,
            IUserRoleRepository userRoleRepository, 
            IRoleRepository roleRepository)
        {
            _logger = logger;
            _localizer = localizer;
            _tokenValidationParams = tokenValidationParams;
            _refreshTokenRepository = refreshTokenRepository;
            _vLoginRepository = vLoginRepository;
            _userRepository = userRepository;
            _standardAccountRepository = standardAccountRepository;
            _emailAccountRepository = emailAccountRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        private async Task<JwtToken> GenerateJwtToken(UserInfo userInfo)
        {
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
                    new Claim(JwtClaimNames.Email, userInfo.Email ?? string.Empty),
                    new Claim(JwtClaimNames.AccountName, userInfo.AccountName ?? string.Empty)
                }),
                // 比较合理的值为 5~10 分钟
                Expires = DateTime.UtcNow.AddSeconds(10),
                SigningCredentials = new SigningCredentials(_tokenValidationParams.IssuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var accessToken = jwtTokenHandler.WriteToken(token);

            var utcNow = DateTime.UtcNow;
            var refreshToken = new T_RefreshToken()
            {
                Id = Guid.NewGuid().ToString("N") + CommonHelper.GetRandomString(24),
                JwtId = token.Id,
                IsRevorked = false,
                Fk_UserId = userInfo.UserId,
                CreatedUtcDate = utcNow,
                ExpiryDate = utcNow.AddMonths(1)
            };

            // 如果您只希望用户仅在一台设备上处于活动状态，则无需将多个刷新令牌存储在存储库中
            await _refreshTokenRepository.DeleteByIdAsync(nameof(T_RefreshToken.Fk_UserId), userInfo.UserId);
            await _refreshTokenRepository.InsertAsync(refreshToken);

            return new JwtToken()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Id
            };
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<AuthResult> Login([FromBody] LoginModel model)
        {
            // 判断用户是否存在
            var vLogin = await _vLoginRepository.QueryByLoginNameAsync(model.LoginName);
            if(vLogin == null)
            {
                return new AuthResult()
                {
                    Message = _localizer["LoginFailed_UserNotFound"]
                };
            }

            var standardAccount = (await _standardAccountRepository.QueryByIdAsync(nameof(T_StandardAccount.Fk_UserId), vLogin.UserId)).FirstOrDefault();
            
            // 检查密码
            string password = StringExtension.FormBase64(model.PasswordHash);
            bool right = CryptoTools.PBKDF2.VerifyPassword(password, standardAccount.PasswordHash, standardAccount.PasswordSalt);
            if (right == false)
            {
                return new AuthResult()
                {
                    Message = _localizer["LoginFailed_PasswordError"]
                };
            }

            // 判断是否是黑名单用户
            if (vLogin.RoleName == Roles.BlacklistUser)
            {
                return new AuthResult()
                {
                    Message = _localizer["LoginFailed_BlacklistUser"]
                };
            }

            await _userRepository.UpdateLoginStateAsync(vLogin.UserId, DateTime.Now, HttpContext.GetRemoteIpAddress());

            var jwtToken = await GenerateJwtToken(new UserInfo
            {
                UserId = vLogin.UserId,
                RoleId = vLogin.RoleId,
                DisplayName = vLogin.DisplayName,
                Email = vLogin.Email,
                AccountName = vLogin.AccountName
            });

            return new AuthResult()
            {
                Code = CustomStatusCode.Succeeded,
                Data = jwtToken
            };
        }

        /// <summary>
        /// RegisterByEmail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<AuthResult> RegisterByEmail([FromBody] EmailRegisterModel model)
        {
            // 检查使用相同电子邮箱的用户是否存在
            bool isExistingEmail = (await _emailAccountRepository.QueryRecordCountAsync("Email=@Email", new { Email = model.Email })) > 0;
            if (isExistingEmail)
            {
                return new AuthResult()
                {
                    Message = "Email already in use"
                };
            }

            // 检查使用相同标准账户名的用户是否存在
            bool isExistingAccountName = (await _standardAccountRepository.QueryRecordCountAsync("AccountName=@AccountName", new { AccountName = model.AccountName })) > 0;
            if (isExistingAccountName)
            {
                return new AuthResult()
                {
                    Message = "Account name already in use"
                };
            }

            var user = new T_User()
            {
                Id = Guid.NewGuid().ToString(),
                DisplayName = model.DisplayName,
                LastLoginIpAddress = HttpContext.GetRemoteIpAddress(),
                LastLoginTime = DateTime.Now
            };

            await _userRepository.InsertAsync(user);

            await _emailAccountRepository.InsertAsync(new T_EmailAccount() 
            {
                Fk_UserId = user.Id,
                Email = model.Email
            });

            string roleId = await _roleRepository.QueryIdByNameAsync(Roles.NormalUser);
            var userRole = new T_UserRole()
            {
                Fk_UserId = user.Id,
                Fk_RoleId = roleId
            };
            await _userRoleRepository.InsertAsync(userRole);


            var jwtToken = await GenerateJwtToken(new UserInfo
            {
                UserId = user.Id,
                RoleId = roleId,
                DisplayName = model.DisplayName,
                Email = model.Email,
                AccountName = model.AccountName
            });

            return new AuthResult()
            {
                Code = CustomStatusCode.Succeeded,
                Data = jwtToken
            };
        }

        /// <summary>
        /// RefreshToken
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<AuthResult> RefreshToken([FromBody] JwtToken token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;

            try
            {
                // Validation 1 - Validation JWT token format
                // 此验证功能将确保 Token 满足验证参数，并且它是一个真正的 token 而不仅仅是随机字符串

                var tokenInVerification = jwtTokenHandler.ValidateToken(token.AccessToken, _tokenValidationParams, out validatedToken);

                return new AuthResult()
                {
                    Message = "Token has not yet expired"
                };
            }
            catch (SecurityTokenExpiredException)
            {
                JwtSecurityToken jwtSecurityToken = validatedToken as JwtSecurityToken;

                // Validation 2 - Validate encryption alg
                // 检查 token 是否有有效的安全算法
                if (validatedToken == null 
                    || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    return new AuthResult()
                    {
                        Message = "Invalid tokens"
                    };
                }

                // validation 3 - validate existence of the token
                // 验证 refresh token 是否存在，是否是保存在数据库的 refresh token
                var storedRefreshToken = (await _refreshTokenRepository.QueryByIdAsync(nameof(T_RefreshToken.Id), token.RefreshToken)).FirstOrDefault();

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

                // Validation 6 - validate if revoked
                // 检查 refresh token 是否被撤销
                if (storedRefreshToken.IsRevorked)
                {
                    return new AuthResult()
                    {
                        Message = "Refresh Token has been revoked"
                    };
                }

                // Validation 7 - validate the id
                // 根据数据库中保存的 Id 验证收到的 token 的 Id
                if (storedRefreshToken.JwtId != validatedToken.Id)
                {
                    return new AuthResult()
                    {
                        Message = "The token doesn't mateched the saved token"
                    };
                }
                
                var userInfo = new UserInfo();
                JwtAuthorizationHandler.ClaimsToUserInfo(jwtSecurityToken.Claims, userInfo);
                // 生成一个新的 token
                var jwtToken = await GenerateJwtToken(userInfo);

                return new AuthResult()
                {
                    Code = CustomStatusCode.Succeeded,
                    Data = jwtToken
                };
            }
            catch (Exception)
            {
                return new AuthResult()
                {
                    Message = "Signature validation failed"
                };
            }
        }

    }
}
