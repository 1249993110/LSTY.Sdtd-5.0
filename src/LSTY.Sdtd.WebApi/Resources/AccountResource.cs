using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LSTY.Sdtd.WebApi.Resources
{
    public struct AccountResource
    {
        public const string LoginFailed_UserNotFound = nameof(LoginFailed_UserNotFound);
        public const string LoginFailed_PasswordError = nameof(LoginFailed_PasswordError);
        public const string LoginFailed_BlacklistUser = nameof(LoginFailed_BlacklistUser);
    }
}
