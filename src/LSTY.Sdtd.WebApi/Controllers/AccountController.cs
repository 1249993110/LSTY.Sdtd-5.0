using IceCoffee.AspNetCore;
using IceCoffee.AspNetCore.Authorization;
using IceCoffee.AspNetCore.Controllers;
using IceCoffee.AspNetCore.Extensions;
using IceCoffee.AspNetCore.Jwt;
using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Models.Primitives;
using IceCoffee.AspNetCore.Options;
using IceCoffee.AspNetCore.Services;
using IceCoffee.Common;
using IceCoffee.Common.Extensions;
using IceCoffee.Common.Security.Cryptography;
using IceCoffee.DbCore.UnitWork;
using LSTY.Sdtd.WebApi.Data;
using LSTY.Sdtd.WebApi.Data.Entities;
using LSTY.Sdtd.WebApi.Data.IRepositories;
using LSTY.Sdtd.WebApi.Data.Primitives;
using LSTY.Sdtd.WebApi.Models.Account;
using LSTY.Sdtd.WebApi.Resources;
using LSTY.Sdtd.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Controllers
{
    /// <summary>
    /// AccountController
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : ApiControllerBase
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
        private readonly ICaptchaGenerator _captchaGenerator;
        private readonly IMemoryCache _memoryCache;
        private readonly EmailService _emailService;
        private readonly UserInfo _userInfo;
        private readonly IVUserDetailsRepository _vUserDetailsRepository;

        public AccountController(ILogger<AccountController> logger,
            IStringLocalizer<AccountResource> localizer,
            TokenValidationParameters tokenValidationParams,
            IRefreshTokenRepository refreshTokenRepository,
            IVLoginRepository vLoginRepository,
            IUserRepository userRepository,
            IStandardAccountRepository standardAccountRepository,
            IEmailAccountRepository emailAccountRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            ICaptchaGenerator captchaGenerator,
            IMemoryCache memoryCache,
            EmailService emailService,
            UserInfo userInfo, 
            IVUserDetailsRepository vUserDetailsRepository)
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
            _captchaGenerator = captchaGenerator;
            _memoryCache = memoryCache;
            _emailService = emailService;
            _userInfo = userInfo;
            _vUserDetailsRepository = vUserDetailsRepository;
        }

        private string GetRegisterCaptchaKey()
        {
            return "RegisterCaptcha:" + HttpContext.Connection.Id;
        }

        private string GetLoginCaptchaKey()
        {
            return "LoginCaptcha:" + HttpContext.Connection.Id;
        }

        /// <summary>
        /// 获取注册验证码，纯数字
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [DevelopmentResponseType(typeof(ResponseResult<RegisterCaptchaModel>), StatusCodes.Status200OK)]
        public async Task<IResponseResult> RegisterCaptcha([FromBody] EmailRegisterModelBase model)
        {
            try
            {
                string key = GetRegisterCaptchaKey();
                if (_memoryCache.TryGetValue(key, out _))
                {
                    return FailedResult(_localizer["RequestsTooFrequent"]);
                }

                RegisterCaptchaModel captchaModel = new RegisterCaptchaModel()
                {
                    ValidSeconds = 60,
                    RequestInterval = 60
                };

                string captcha = CommonHelper.GetRandomString("0123456789", 6);

                _memoryCache.Set(key, captcha, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60D)
                });

                await _emailService.SendAsync(new EmailSendOptions()
                {
                    Captcha = captcha,
                    CurrentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FromAddress = "lsty@7daystodie.top",
                    FromDisplayName = "洛水天依",
                    IsBodyHtml = true,
                    Subject = "洛水天依 验证码",
                    TemplateFilePath = "email-template.zh-CN.html",
                    ToAddress = model.Email,
                    ToDisplayName = model.DisplayName,
                    AccountName = model.AccountName
                });

                return new ResponseResult<RegisterCaptchaModel>()
                {
                    Code = CustomStatusCode.OK,
                    Data = captchaModel
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邮件发送失败");
                return FailedResult(_localizer["SendMailFailed"]);
            }
        }

        /// <summary>
        /// 获取登录验证码，纯数字
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public Task<ResponseResult<LoginCaptchaModel>> LoginCaptcha()
        {
            var captchaCode = CommonHelper.GetRandomString("0123456789", 4);
            var imgByte = _captchaGenerator.GenerateImageAsByteArray(captchaCode);

            LoginCaptchaModel captchaModel = new LoginCaptchaModel()
            {
                RequestInterval = 180,
                ValidSeconds = 180,
                ImageBase64 = "data:image/png;base64," + Convert.ToBase64String(imgByte)
            };

            string key = GetLoginCaptchaKey();
            _memoryCache.Set(key, captchaCode, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(180D)
            });

            return Task.FromResult(new ResponseResult<LoginCaptchaModel>()
            {
                Code = CustomStatusCode.OK,
                Data = captchaModel
            });
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseResult> Login([FromBody] LoginModel model)
        {
            string key = GetLoginCaptchaKey();
            // 验证码过期
            if (_memoryCache.TryGetValue(key, out string cachedCaptchaCode) == false)
            {
                return FailedResult(_localizer["LoginFailed_InvalidCaptcha"]);
            }
            else
            {
                // 验证码错误
                if(model.CaptchaCode != cachedCaptchaCode)
                {
                    return FailedResult(_localizer["LoginFailed_InvalidCaptcha"]);
                }
                else
                {
                    _memoryCache.Remove(key);
                }
            }

            // 判断用户是否存在
            var vLogin = await _vLoginRepository.QueryByLoginNameAsync(model.LoginName);
            if(vLogin == null)
            {
                return FailedResult(_localizer["LoginFailed_UserNotFound"]);
            }

            var standardAccount = (await _standardAccountRepository.QueryByIdAsync(nameof(T_StandardAccount.Fk_UserId), vLogin.UserId)).FirstOrDefault();
            
            // 检查密码
            string password = StringExtension.FormBase64(model.PasswordHash);
            bool right = CryptoTools.PBKDF2.VerifyPassword(password, standardAccount.PasswordHash, standardAccount.PasswordSalt);
            if (right == false)
            {
                return FailedResult(_localizer["LoginFailed_PasswordError"]);
            }

            // 判断是否是黑名单用户
            if (vLogin.RoleName == Roles.BlacklistUser)
            {
                return FailedResult(_localizer["LoginFailed_BlacklistUser"]);
            }

            await _userRepository.UpdateLoginStateAsync(vLogin.UserId, DateTime.Now, HttpContext.GetRemoteIpAddress());

            var jwtToken = await GenerateJwtToken(new UserInfo
            {
                UserId = vLogin.UserId.ToString(),
                RoleId = vLogin.RoleId.ToString(),
                RoleName = vLogin.RoleName,
                DisplayName = vLogin.DisplayName,
                Email = vLogin.Email,
                AccountName = vLogin.AccountName
            });

            return SucceededResult(jwtToken);
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseResult> Logout()
        {
            await _refreshTokenRepository.DeleteByIdAsync(nameof(T_RefreshToken.Fk_UserId), _userInfo.UserId);
            return SucceededResult();
        }

        /// <summary>
        /// RegisterByEmail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseResult> RegisterByEmail([FromBody] EmailRegisterModel model)
        {
            string key = GetRegisterCaptchaKey();
            // 验证码过期
            if (_memoryCache.TryGetValue(key, out string captchaCode) == false)
            {
                return FailedResult(_localizer["RegisterFailed_InvalidCaptcha"]);
            }
            else
            {
                // 验证码错误
                if (model.CaptchaCode != captchaCode)
                {
                    return FailedResult(_localizer["RegisterFailed_InvalidCaptcha"]);
                }
                else
                {
                    _memoryCache.Remove(key);
                }
            }

            // 检查使用相同电子邮箱的用户是否存在
            bool isExistingEmail = await _emailAccountRepository.IsExist(model.Email);
            if (isExistingEmail)
            {
                return FailedResult(_localizer["RegisterFailed_EmailHasUsed"]);
            }
            
            // 检查账户名是否规范
            bool accountNameAllow = new Regex(@"^[a-zA-Z0-9]{6,20}$").IsMatch(model.AccountName);
            if (accountNameAllow == false)
            {
                return FailedResult(_localizer["RegisterFailed_InvalidAccountName"]);
            }

            string password = StringExtension.FormBase64(model.PasswordHash);
            // 检查密码强度
            bool passwordAllow = new Regex(@"^[\x21-\x7e]{6,20}$").IsMatch(password);
            if (passwordAllow == false)
            {
                return FailedResult(_localizer["RegisterFailed_InvalidPassword"]);
            }

            // 检查使用相同标准账户名的用户是否存在
            bool isExistingAccountName = await _standardAccountRepository.IsExist(model.AccountName);
            if (isExistingAccountName)
            {
                return FailedResult(_localizer["RegisterFailed_AccountNameHasUsed"]);
            }

            Guid roleId = await _roleRepository.QueryIdByNameAsync(Roles.NormalUser);

            var user = new T_User()
            {
                Id = Guid.NewGuid(),
                DisplayName = model.DisplayName,
                LastLoginIpAddress = HttpContext.GetRemoteIpAddress(),
                LastLoginTime = DateTime.Now
            };

            CryptoTools.PBKDF2.HashPassword(password, out string passwordHash, out string passwordSalt);

            var standardAccount = new T_StandardAccount()
            {
                Fk_UserId = user.Id,
                AccountName = model.AccountName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            var unitOfWork = UnitOfWork.Default.EnterContext(ConnectionInfoManager.DefaultConnectionInfo);

            try
            {
                _userRepository.Insert(user);

                _standardAccountRepository.Insert(standardAccount);

                _emailAccountRepository.Insert(new T_EmailAccount()
                {
                    Fk_UserId = user.Id,
                    Email = model.Email
                });

                var userRole = new T_UserRole()
                {
                    Fk_UserId = user.Id,
                    Fk_RoleId = roleId
                };

                _userRoleRepository.Insert(userRole);

                unitOfWork.SaveChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            var jwtToken = await GenerateJwtToken(new UserInfo
            {
                UserId = user.Id.ToString(),
                RoleId = roleId.ToString(),
                RoleName = Roles.NormalUser,
                DisplayName = model.DisplayName,
                Email = model.Email,
                AccountName = model.AccountName
            });

            return SucceededResult(jwtToken);
        }

        /// <summary>
        /// RefreshToken
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseResult> RefreshToken([FromBody] JwtToken token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;

            try
            {
                // Validation 1 - Validation JWT token format
                // 此验证功能将确保 Token 满足验证参数，并且它是一个真正的 token 而不仅仅是随机字符串

                var tokenInVerification = jwtTokenHandler.ValidateToken(token.AccessToken, _tokenValidationParams, out validatedToken);

                return FailedResult(_localizer["RefreshTokenFailed_TokenNotExpired"]);
            }
            catch (SecurityTokenExpiredException)
            {
                JwtSecurityToken jwtSecurityToken = validatedToken as JwtSecurityToken;

                // Validation 2 - Validate encryption alg
                // 检查 token 是否有有效的安全算法
                if (validatedToken == null 
                    || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    return FailedResult(_localizer["RefreshTokenFailed_InvalidToken"]);
                }

                // validation 3 - validate existence of the token
                // 验证 refresh token 是否存在，是否是保存在数据库的 refresh token
                var storedRefreshToken = (await _refreshTokenRepository.QueryByIdAsync(nameof(T_RefreshToken.Id), token.RefreshToken)).FirstOrDefault();

                if (storedRefreshToken == null)
                {
                    return FailedResult(_localizer["RefreshTokenFailed_TokenNotExist"]);
                }

                // Validation 5 - 检查存储的 RefreshToken 是否已过期
                // Check the date of the saved refresh token if it has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return FailedResult(_localizer["RefreshTokenFailed_TokenExpired"]);
                }

                // Validation 6 - validate if revoked
                // 检查 refresh token 是否被撤销
                if (storedRefreshToken.IsRevorked)
                {
                    return FailedResult(_localizer["RefreshTokenFailed_TokenHasRevoked"]);
                }

                // Validation 7 - validate the id
                // 根据数据库中保存的 Id 验证收到的 token 的 Id
                if (storedRefreshToken.JwtId.ToString() != validatedToken.Id)
                {
                    return FailedResult(_localizer["RefreshTokenFailed_TokenNotMateched"]);
                }
                
                var userInfo = new UserInfo();
                JwtAuthorizationHandler.ClaimsToUserInfo(jwtSecurityToken.Claims, ref userInfo);
                // 生成一个新的 token
                var jwtToken = await GenerateJwtToken(userInfo);

                return SucceededResult(jwtToken);
            }
            catch (Exception)
            {
                return FailedResult(_localizer["RefreshTokenFailed_InvalidSignature"]);
            }
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
                    new Claim(JwtClaimNames.UserId, userInfo.UserId.ToString()),
                    new Claim(JwtClaimNames.RoleId, userInfo.RoleId.ToString()),
                    new Claim(JwtClaimNames.RoleName, userInfo.RoleName),
                    new Claim(JwtClaimNames.DisplayName, userInfo.DisplayName),
                    new Claim(JwtClaimNames.Email, userInfo.Email ?? string.Empty),
                    new Claim(JwtClaimNames.AccountName, userInfo.AccountName ?? string.Empty)
                }),
                // 比较合理的值为 5~10 分钟
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(_tokenValidationParams.IssuerSigningKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var accessToken = jwtTokenHandler.WriteToken(token);

            var utcNow = DateTime.UtcNow;
            var refreshToken = new T_RefreshToken()
            {
                Id = Guid.NewGuid().ToString("N") + CommonHelper.GetRandomString(24),
                JwtId = Guid.Parse(token.Id),
                IsRevorked = false,
                Fk_UserId = Guid.Parse(userInfo.UserId),
                CreatedUtcDate = utcNow,
                ExpiryDate = utcNow.AddDays(7)
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
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseResult> ChangePassword([FromBody] ChangePasswordParams model)
        {
            var user = (await _standardAccountRepository.QueryByIdAsync(nameof(T_StandardAccount.Fk_UserId), _userInfo.UserId)).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("修改密码异常，用户已被删除或异常访问");
            }

            string oldPassword = StringExtension.FormBase64(model.OldPasswordHash);
            string newPassword = StringExtension.FormBase64(model.NewPasswordHash);

            // 检查密码
            bool right = CryptoTools.PBKDF2.VerifyPassword(oldPassword, user.PasswordHash, user.PasswordSalt);

            if (right == false)
            {
                return FailedResult(_localizer["ChangePasswordFailed"]);
            }

            CryptoTools.PBKDF2.HashPassword(newPassword, out string hashValue, out string saltBase64);
            user.PasswordHash = hashValue;
            user.PasswordSalt = saltBase64;

            await _standardAccountRepository.UpdateAsync(user);

            await _refreshTokenRepository.DeleteByIdAsync(nameof(T_RefreshToken.Fk_UserId), _userInfo.UserId);
            return SucceededResult();
        }

        /// <summary>
        /// 获取当前用户资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseResult> Profile()
        {
           var user = (await  _vUserDetailsRepository.QueryByIdAsync(nameof(V_UserDetails.UserId), _userInfo.UserId)).FirstOrDefault();
            return new ResponseResult() 
            { 
                Code = CustomStatusCode.OK,
                Data = user
            };
        }
    }
}
