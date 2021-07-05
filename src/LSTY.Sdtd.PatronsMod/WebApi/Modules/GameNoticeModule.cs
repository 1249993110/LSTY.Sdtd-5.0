using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
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
        public GameNoticeModule() : base("/GameNotice")
        {
            HttpGet("/RetrieveGameNoticeConfig", "RetrieveGameNoticeConfig", _ =>
            {
                var gameNotice = FunctionManager.GameNotice;
                var data = new GameNoticeConfigViewModel()
                {
                    AlternateInterval = gameNotice.AlternateInterval,
                    AlternateNotice = gameNotice.AlternateNotice,
                    AlternateNotice1 = gameNotice.AlternateNotice1,
                    AlternateNotice2 = gameNotice.AlternateNotice2,
                    AlternateNotice3 = gameNotice.AlternateNotice3,
                    AlternateNotice4 = gameNotice.AlternateNotice4,
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
                gameNotice.AlternateNotice1 = data.AlternateNotice1;
                gameNotice.AlternateNotice2 = data.AlternateNotice2;
                gameNotice.AlternateNotice3 = data.AlternateNotice3;
                gameNotice.AlternateNotice4 = data.AlternateNotice4;
                gameNotice.WelcomeNotice = data.WelcomeNotice;

                ConfigManager.Save(gameNotice);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables", "RetrieveAvailableVariables_GameNotice", _ =>
            {
                return SucceededResult(FunctionManager.GameNotice.AvailableVariables);
            });
        }
    }
}
