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
    public class TeleportCityModule : ApiModuleBase
    {
        private static readonly ICityPositionRepository _cityPositionRepository;

        static TeleportCityModule()
        {
            _cityPositionRepository = IocContainer.Resolve<ICityPositionRepository>();
        }

        public TeleportCityModule()
        {
            HttpGet("/RetrieveTeleportCityConfig", "RetrieveTeleportCityConfig", _ =>
            {
                var function = FunctionManager.TeleportCity;
                var data = new TeleportCityConfigViewModel()
                {
                    QueryListPreTips = function.QueryListPreTips,
                    FunctionName = function.FunctionName,
                    IsEnabled = function.IsEnabled,
                    NoneCityTips = function.NoneCityTips,
                    QueryListCmd = function.QueryListCmd,
                    QueryListTips = function.QueryListTips,
                    PointsNotEnoughTips = function.PointsNotEnoughTips,
                    CoolingTips = function.CoolingTips,
                    TeleInterval = function.TeleInterval,
                    TeleSucceedTips = function.TeleSucceedTips
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateTeleportCityConfig", "UpdateTeleportCityConfig", _ =>
            {
                var data = this.Bind<TeleportCityConfigViewModel>();
                var function = FunctionManager.TeleportCity;

                function.QueryListPreTips = data.QueryListPreTips;
                function.IsEnabled = data.IsEnabled;
                function.NoneCityTips = data.NoneCityTips;
                function.QueryListCmd = data.QueryListCmd;
                function.QueryListTips = data.QueryListTips;
                function.PointsNotEnoughTips = data.PointsNotEnoughTips;
                function.CoolingTips = data.CoolingTips;
                function.TeleInterval = data.TeleInterval;
                function.TeleSucceedTips = data.TeleSucceedTips;

                ConfigManager.Save(function);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables_TeleportCity", "RetrieveAvailableVariables_TeleportCity", _ =>
            {
                return SucceededResult(FunctionManager.TeleportCity.AvailableVariables);
            });

            HttpPost("/CreateCityPosition", "CreateCityPosition", _ =>
            {
                var data = this.Bind<CityPositionViewModel>();
                _cityPositionRepository.Insert(new T_CityPosition() 
                {
                    Id = Guid.NewGuid().ToString(),
                    CityName = data.CityName,
                    Command = data.Command,
                    PointsRequired = data.PointsRequired,
                    Position = data.Position
                });

                return SucceededResult();
            });

            HttpGet("/RetrieveCityPosition", "RetrieveCityPosition", _ =>
            {
                string cityPositionId = Request.Query["cityPositionId"];
                if (string.IsNullOrEmpty(cityPositionId))
                {
                    var cityPositions = _cityPositionRepository.QueryAll();
                    return SucceededResult(cityPositions);
                }

                var data = _cityPositionRepository.QueryById(nameof(T_CityPosition.Id), cityPositionId);

                if (data == null || data.Any() == false)
                {
                    return FailedResult(message: "The specified cityPositionId does not exist");
                }

                return SucceededResult(data);
            });

            HttpPost("/UpdateCityPosition", "UpdateCityPosition", _ =>
            {
                var data = this.Bind<CityPositionViewModel>();

                var entity = new T_CityPosition()
                {
                    Id = data.Id,
                    CityName = data.CityName,
                    Command = data.Command,
                    PointsRequired = data.PointsRequired,
                    Position = data.Position
                };

                int count = _cityPositionRepository.Update(entity);

                if (count != 1)
                {
                    return FailedResult(message: "The specified cityPositionId does not exist");
                }

                return SucceededResult();
            });

            HttpPost("/DeleteCityPosition", "DeleteCityPosition", _ =>
            {
                var data = this.Bind<DeleteQueryParamOfString>();

                if (data == null || data.Ids == null || data.Ids.Any() == false)
                {
                    return FailedResult(message: "The specified cityPositionIds is null");
                }

                int count = _cityPositionRepository.DeleteBatchByIds(nameof(T_CityPosition.Id), data.Ids);

                if (count != data.Ids.Length)
                {
                    return FailedResult(message: "Deleted count: " + count);
                }

                return SucceededResult();
            });
        }
    }
}
