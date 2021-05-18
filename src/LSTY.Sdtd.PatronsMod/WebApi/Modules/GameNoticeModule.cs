using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.WebApi.ViewModels;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class GameNoticeModule : ApiModuleBase
    {
        public GameNoticeModule()
        {
            HttpGet("/RetrieveGameNoticeConfig", "RetrieveGameNoticeConfig", _ =>
            {
                var gameNotice = FunctionManager.GameNotice;
                var data = new GameNoticeConfigViewModel()
                {
                    AlternateInterval = gameNotice.AlternateInterval,
                    AlternateNotice = gameNotice.AlternateNotice,
                    FunctionName = gameNotice.FunctionName,
                    IsEnabled = gameNotice.IsEnabled,
                    WelcomeNotice = gameNotice.WelcomeNotice
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateGameNoticeConfig", "UpdateGameNoticeConfig", _ =>
            {
                var data = this.Bind<GameNoticeConfigViewModel>();
                var gameNotice = FunctionManager.GameNotice;
                gameNotice.IsEnabled = data.IsEnabled;
                gameNotice.AlternateInterval = data.AlternateInterval;
                gameNotice.AlternateNotice = data.AlternateNotice;
                gameNotice.WelcomeNotice = data.WelcomeNotice;

                ConfigManager.Save(gameNotice);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables_GameNotice", "RetrieveAvailableVariables_GameNotice", _ =>
            {
                return SucceededResult(FunctionManager.GameNotice.AvailableVariables);
            });
        }
    }
}
