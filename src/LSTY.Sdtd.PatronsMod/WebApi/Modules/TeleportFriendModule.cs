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
    public class TeleportFriendModule : ApiModuleBase
    {
        public TeleportFriendModule() : base("/TeleportFriend")
        {
            HttpGet("/RetrieveTeleportFriendConfig", "RetrieveTeleportFriendConfig", _ =>
            {
                var function = FunctionManager.TeleportFriend;
                var data = new TeleportFriendConfigViewModel()
                {
                    FunctionName = function.FunctionName,
                    IsEnabled = function.IsEnabled,
                    CoolingTips = function.CoolingTips,
                    PointsNotEnoughTips = function.PointsNotEnoughTips,
                    PointsRequired = function.PointsRequired,
                    TargetNotFoundTips = function.TargetNotFoundTips,
                    TargetNotFriendTips = function.TargetNotFriendTips,
                    TeleCmd = function.TeleCmd,
                    TeleInterval = function.TeleInterval,
                    TeleSucceedTips = function.TeleSucceedTips
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateTeleportFriendConfig", "UpdateTeleportFriendConfig", _ =>
            {
                var data = this.Bind<TeleportFriendConfigViewModel>();
                var function = FunctionManager.TeleportFriend;
                function.IsEnabled = data.IsEnabled;
                function.CoolingTips = data.CoolingTips;
                function.PointsNotEnoughTips = data.PointsNotEnoughTips;
                function.PointsRequired = data.PointsRequired;
                function.TargetNotFoundTips = data.TargetNotFoundTips;
                function.TargetNotFriendTips = data.TargetNotFriendTips;
                function.TeleCmd = data.TeleCmd;
                function.TeleInterval = data.TeleInterval;
                function.TeleSucceedTips = data.TeleSucceedTips;

                ConfigManager.Save(function);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables", "RetrieveAvailableVariables_TeleportFriend", _ =>
            {
                return SucceededResult(FunctionManager.TeleportFriend.AvailableVariables);
            });
        }
    }
}
