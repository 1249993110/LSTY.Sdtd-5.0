using Hei.Captcha;
using IceCoffee.AspNetCore;
using IceCoffee.AspNetCore.Authorization;
using IceCoffee.AspNetCore.Extensions;
using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Models.Results;
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
        private readonly SecurityCodeHelper _securityCode;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService;
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
            SecurityCodeHelper securityCode,
            IMemoryCache memoryCache,
            IEmailService emailService,
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
            _securityCode = securityCode;
            _memoryCache = memoryCache;
            _emailService = emailService;
            _userInfo = userInfo;
            _vUserDetailsRepository = vUserDetailsRepository;
        }

        private string GetRegisterCaptchaKey(string registerName)
        {
            return "RegisterCaptcha:" + HttpContext.GetRemoteIpAddress() + ":" + registerName;
        }

        private string GetLoginCaptchaKey(string loginName)
        {
            return "LoginCaptcha:" + HttpContext.GetRemoteIpAddress() + ":" + loginName;
        }

        /// <summary>
        /// 获取注册验证码，纯数字
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [DevelopmentResponseType(typeof(RegisterCaptchaModel))]
        public async Task<IRespResult> RegisterCaptcha([FromBody] EmailRegisterModelBase model)
        {
            try
            {
                string key = GetRegisterCaptchaKey(model.Email);
                if (_memoryCache.TryGetValue(key, out _))
                {
                    return new FailedResult(HttpContext.GetRequestId())
                    {
                        Message = _localizer["RequestsTooFrequent"]
                    };
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

                await _emailService.SendAsync(new EmailSendParams()
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

                return new RespResult<RegisterCaptchaModel>()
                {
                    Code = CustomStatusCode.Succeeded,
                    Data = captchaModel
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邮件发送失败");
                return new FailedResult(HttpContext.GetRequestId())
                {
                    Message = _localizer["SendMailFailed"]
                };
            }
        }

        /// <summary>
        /// 获取登录验证码，数字字母组合
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public RespResult<LoginCaptchaModel> LoginCaptcha([FromBody] LoginModelBase model)
        {
            var captcha = CommonHelper.GetRandomString("0123456789", 4);
            var imgbyte = _securityCode.GetEnDigitalCodeByte(captcha);

            LoginCaptchaModel captchaModel = new LoginCaptchaModel()
            {
                RequestInterval = 180,
                ValidSeconds = 180,
                ImageBase64 = "data:image/png;base64," + Convert.ToBase64String(imgbyte)
            };

            string key = GetLoginCaptchaKey(model.LoginName);
            _memoryCache.Set(key, captcha, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(180D)
            });

            return new RespResult<LoginCaptchaModel>()
            {
                Code = CustomStatusCode.Succeeded,
                Data = captchaModel
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
            string key = GetLoginCaptchaKey(model.LoginName);
            // 验证码过期
            if (_memoryCache.TryGetValue(key, out string captcha) == false)
            {
                return new AuthResult()
                {
                    Message = _localizer["LoginFailed_InvalidCaptcha"]
                };
            }
            else
            {
                // 验证码错误
                if(model.Captcha != captcha)
                {
                    return new AuthResult()
                    {
                        Message = _localizer["LoginFailed_InvalidCaptcha"]
                    };
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
        /// Logout
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<SucceededNullResult> Logout()
        {
            await _refreshTokenRepository.DeleteByIdAsync(nameof(T_RefreshToken.Fk_UserId), _userInfo.UserId);
            return new SucceededNullResult();
        }

        /// <summary>
        /// RegisterByEmail
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<AuthResult> RegisterByEmail([FromBody] EmailRegisterModel model)
        {
            string key = GetRegisterCaptchaKey(model.Email);
            // 验证码过期
            if (_memoryCache.TryGetValue(key, out string captcha) == false)
            {
                return new AuthResult()
                {
                    Message = _localizer["RegisterFailed_InvalidCaptcha"]
                };
            }
            else
            {
                // 验证码错误
                if (model.Captcha != captcha)
                {
                    return new AuthResult()
                    {
                        Message = _localizer["RegisterFailed_InvalidCaptcha"]
                    };
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
                return new AuthResult()
                {
                    Message = _localizer["RegisterFailed_EmailHasUsed"]
                };
            }
            
            // 检查账户名是否规范
            bool accountNameAllow = new Regex(@"^[a-zA-Z0-9]{6,20}$").IsMatch(model.AccountName);
            if (accountNameAllow == false)
            {
                return new AuthResult()
                {
                    Message = _localizer["RegisterFailed_InvalidAccountName"]
                };
            }

            string password = StringExtension.FormBase64(model.PasswordHash);
            // 检查密码强度
            bool passwordAllow = new Regex(@"^[\x21-\x7e]{6,20}$").IsMatch(password);
            if (passwordAllow == false)
            {
                return new AuthResult()
                {
                    Message = _localizer["RegisterFailed_InvalidPassword"]
                };
            }

            // 检查使用相同标准账户名的用户是否存在
            bool isExistingAccountName = await _standardAccountRepository.IsExist(model.AccountName);
            if (isExistingAccountName)
            {
                return new AuthResult()
                {
                    Message = _localizer["RegisterFailed_AccountNameHasUsed"]
                };
            }

            string roleId = await _roleRepository.QueryIdByNameAsync(Roles.NormalUser);

            var user = new T_User()
            {
                Id = Guid.NewGuid().ToString(),
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
                    Message = _localizer["RefreshTokenFailed_TokenNotExpired"]
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
                        Message = _localizer["RefreshTokenFailed_InvalidToken"]
                    };
                }

                // validation 3 - validate existence of the token
                // 验证 refresh token 是否存在，是否是保存在数据库的 refresh token
                var storedRefreshToken = (await _refreshTokenRepository.QueryByIdAsync(nameof(T_RefreshToken.Id), token.RefreshToken)).FirstOrDefault();

                if (storedRefreshToken == null)
                {
                    return new AuthResult()
                    {
                        Message = _localizer["RefreshTokenFailed_TokenNotExist"]
                    };
                }

                // Validation 5 - 检查存储的 RefreshToken 是否已过期
                // Check the date of the saved refresh token if it has expired
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AuthResult()
                    {
                        Message = _localizer["RefreshTokenFailed_TokenExpired"]
                    };
                }

                // Validation 6 - validate if revoked
                // 检查 refresh token 是否被撤销
                if (storedRefreshToken.IsRevorked)
                {
                    return new AuthResult()
                    {
                        Message = _localizer["RefreshTokenFailed_TokenHasRevoked"]
                    };
                }

                // Validation 7 - validate the id
                // 根据数据库中保存的 Id 验证收到的 token 的 Id
                if (storedRefreshToken.JwtId != validatedToken.Id)
                {
                    return new AuthResult()
                    {
                        Message = _localizer["RefreshTokenFailed_TokenNotMateched"]
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
                    Message = _localizer["RefreshTokenFailed_InvalidSignature"]
                };
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
                    new Claim(JwtClaimNames.UserId, userInfo.UserId),
                    new Claim(JwtClaimNames.RoleId, userInfo.RoleId),
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
                JwtId = token.Id,
                IsRevorked = false,
                Fk_UserId = userInfo.UserId,
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
        [DevelopmentResponseType(typeof(SucceededNullResult))]
        public async Task<IRespResult> ChangePassword([FromBody] ChangePasswordParams model)
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
                return new FailedResult()
                {
                    Message = _localizer["ChangePasswordFailed"]
                };
            }

            CryptoTools.PBKDF2.HashPassword(newPassword, out string hashValue, out string saltBase64);
            user.PasswordHash = hashValue;
            user.PasswordSalt = saltBase64;

            await _standardAccountRepository.UpdateAsync(user);

            await _refreshTokenRepository.DeleteByIdAsync(nameof(T_RefreshToken.Fk_UserId), _userInfo.UserId);
            return new SucceededNullResult();
        }

        /// <summary>
        /// 获取当前用户资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<RespResult<V_UserDetails>> Profile()
        {
           var user = (await  _vUserDetailsRepository.QueryByIdAsync(nameof(V_UserDetails.UserId), _userInfo.UserId)).FirstOrDefault();
            return new RespResult<V_UserDetails>() 
            { 
                Code = CustomStatusCode.Succeeded,
                Data = user
            };
        }
    }
}
