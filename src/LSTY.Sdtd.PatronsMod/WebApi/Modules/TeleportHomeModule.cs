using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
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
    public class TeleportHomeModule : ApiModuleBase
    {
        private static readonly IHomePositionRepository _homePositionRepository;

        static TeleportHomeModule()
        {
            _homePositionRepository = IocContainer.Resolve<IHomePositionRepository>();
        }

        public TeleportHomeModule()
        {
            HttpGet("/RetrieveTeleportHomeConfig", "RetrieveTeleportHomeConfig", _ =>
            {
                var function = FunctionManager.TeleportHome;
                var data = new TeleportHomeConfigViewModel()
                {
                    FunctionName = function.FunctionName,
                    DeleteHomeCmdPrefix = function.DeleteHomeCmdPrefix,
                    IsEnabled = function.IsEnabled,
                    MaxCanSetCount = function.MaxCanSetCount,
                    QueryListCmd = function.QueryListCmd,
                    DeleteSucceedTips = function.DeleteSucceedTips,
                    HomeNotFoundTips = function.HomeNotFoundTips,
                    CoolingTips = function.CoolingTips,
                    TeleInterval = function.TeleInterval,
                    TeleSucceedTips = function.TeleSucceedTips,
                    QueryListTips = function.QueryListTips,
                    NoneHaveHomeTips = function.NoneHaveHomeTips,
                    OverLimitTips = function.OverLimitTips,
                    OverwriteOldSucceedTips = function.OverwriteOldSucceedTips,
                    QueryListPreTips = function.QueryListPreTips,
                    PointsRequiredForSet = function.PointsRequiredForSet,
                    PointsRequiredForTele = function.PointsRequiredForTele,
                    SetHomeCmdPrefix = function.SetHomeCmdPrefix,
                    SetPointsNotEnoughTips = function.SetPointsNotEnoughTips,
                    SetSucceedTips = function.SetSucceedTips,
                    TeleHomeCmdPrefix = function.TeleHomeCmdPrefix,
                    TelePointsNotEnoughTips = function.TelePointsNotEnoughTips
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateTeleportHomeConfig", "UpdateTeleportHomeConfig", _ =>
            {
                var data = this.Bind<TeleportHomeConfigViewModel>();
                var function = FunctionManager.TeleportHome;

                function.DeleteHomeCmdPrefix = data.DeleteHomeCmdPrefix;
                function.IsEnabled = data.IsEnabled;
                function.MaxCanSetCount = data.MaxCanSetCount;
                function.QueryListCmd = data.QueryListCmd;
                function.DeleteSucceedTips = data.DeleteSucceedTips;
                function.HomeNotFoundTips = data.HomeNotFoundTips;
                function.CoolingTips = data.CoolingTips;
                function.TeleInterval = data.TeleInterval;
                function.TeleSucceedTips = data.TeleSucceedTips;
                function.QueryListTips = data.QueryListTips;
                function.NoneHaveHomeTips = data.NoneHaveHomeTips;
                function.OverLimitTips = data.OverLimitTips;
                function.OverwriteOldSucceedTips = data.OverwriteOldSucceedTips;
                function.QueryListPreTips = data.QueryListPreTips;
                function.PointsRequiredForSet = data.PointsRequiredForSet;
                function.PointsRequiredForTele = data.PointsRequiredForTele;
                function.SetHomeCmdPrefix = data.SetHomeCmdPrefix;
                function.SetPointsNotEnoughTips = data.SetPointsNotEnoughTips;
                function.SetSucceedTips = data.SetSucceedTips;
                function.TeleHomeCmdPrefix = data.TeleHomeCmdPrefix;
                function.TelePointsNotEnoughTips = data.TelePointsNotEnoughTips;

                ConfigManager.Save(function);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables_TeleportHome", "RetrieveAvailableVariables_TeleportHome", _ =>
            {
                return SucceededResult(FunctionManager.TeleportHome.AvailableVariables);
            });

            HttpPost("/CreateHomePosition", "CreateHomePosition", _ =>
            {
                var data = this.Bind<HomePositionViewModel>();
                _homePositionRepository.Insert(new T_HomePosition() 
                {
                    Id = Guid.NewGuid().ToString(),
                    HomeName = data.HomeName,
                    SteamId = data.SteamId,
                    Position = data.Position
                });

                return SucceededResult();
            });

            HttpGet("/RetrieveHomePosition", "RetrieveHomePosition", _ =>
            {
                string homePositionId = Request.Query["homePositionId"];
                if (string.IsNullOrEmpty(homePositionId))
                {
                    var homePositions = _homePositionRepository.QueryAll();
                    return SucceededResult(homePositions);
                }

                var data = _homePositionRepository.QueryById(nameof(T_HomePosition.Id), homePositionId);

                if (data == null || data.Any() == false)
                {
                    return FailedResult(message: "The specified homePositionId does not exist");
                }

                return SucceededResult(data);
            });

            HttpPost("/UpdateHomePosition", "UpdateHomePosition", _ =>
            {
                var data = this.Bind<HomePositionViewModel>();

                var entity = new T_HomePosition()
                {
                    Id = data.Id,
                    HomeName = data.HomeName,
                    SteamId = data.SteamId,
                    Position = data.Position
                };

                int count = _homePositionRepository.Update(entity);

                if (count != 1)
                {
                    return FailedResult(message: "The specified homePositionId does not exist");
                }

                return SucceededResult();
            });

            HttpPost("/DeleteHomePosition", "DeleteHomePosition", _ =>
            {
                var data = this.Bind<DeleteQueryParamOfString>();

                if (data == null || data.Ids == null || data.Ids.Any() == false)
                {
                    return FailedResult(message: "The specified homePositionIds is null");
                }

                int count = _homePositionRepository.DeleteBatchByIds(nameof(T_HomePosition.Id), data.Ids);

                if (count != data.Ids.Length)
                {
                    return FailedResult(message: "Deleted count: " + count);
                }

                return SucceededResult();
            });
        }
    }
}
