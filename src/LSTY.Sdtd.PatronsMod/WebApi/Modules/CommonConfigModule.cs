using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class CommonConfigModule : ApiModuleBase
    {
        public CommonConfigModule() : base("/CommonConfig")
        {
            HttpGet("/RetrieveCommonConfig", "RetrieveCommonConfig", _ =>
            {
                var commonConfig = FunctionManager.CommonConfig;
                var data = new CommonConfigViewModel()
                {
                    ChatCommandCacheMaxCount = commonConfig.ChatCommandCacheMaxCount,
                    ChatCommandPrefix = commonConfig.ChatCommandPrefix,
                    FunctionName = commonConfig.FunctionName,
                    HandleChatMessageError = commonConfig.HandleChatMessageErrorTips,
                    ServerName = commonConfig.ServerName,
                    WebConfig = commonConfig.WebConfig
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateCommonConfig", "UpdateCommonConfig", _ =>
            {
                var data = this.Bind<CommonConfigViewModel>();

                var commonConfig = FunctionManager.CommonConfig;
                commonConfig.ChatCommandCacheMaxCount = data.ChatCommandCacheMaxCount;
                commonConfig.ChatCommandPrefix = data.ChatCommandPrefix;
                commonConfig.HandleChatMessageErrorTips = data.HandleChatMessageError;
                commonConfig.ServerName = data.ServerName;
                commonConfig.WebConfig = data.WebConfig;


                ConfigManager.Save(commonConfig);

                return SucceededResult();
            });
        }
    }
}
