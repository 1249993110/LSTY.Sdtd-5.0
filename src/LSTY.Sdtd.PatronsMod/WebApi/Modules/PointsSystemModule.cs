using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class PointsSystemModule : ApiModuleBase
    {
        private static readonly IPointsRepository _pointsRepository;

        static PointsSystemModule()
        {
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();
        }

        public PointsSystemModule()
        {
            HttpGet("/RetrievePointsSystemConfig", "RetrievePointsSystemConfig", _ =>
            {
                var pointsSystem = FunctionManager.PointsSystem;
                var data = new PointsSystemConfigViewModel()
                {
                    NeverSignInTips = pointsSystem.NeverSignInTips,
                    QueryPointsCmd = pointsSystem.QueryPointsCmd,
                    QueryPointsTips = pointsSystem.QueryPointsTips,
                    RewardCount = pointsSystem.RewardCount,
                    SignCmd = pointsSystem.SignCmd,
                    SignFailTips = pointsSystem.SignFailTips,
                    SignSucceedTips = pointsSystem.SignSucceedTips,
                    IsEnabled = pointsSystem.IsEnabled,
                    FunctionName = pointsSystem.FunctionName,
                    InitialCount = pointsSystem.InitialCount,
                    SignInterval = pointsSystem.SignInterval
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdatePointsSystemConfig", "UpdatePointsSystemConfig", _ =>
            {
                var data = this.Bind<PointsSystemConfigViewModel>();
                var pointsSystem = FunctionManager.PointsSystem;

                pointsSystem.IsEnabled = data.IsEnabled;
                pointsSystem.NeverSignInTips = data.NeverSignInTips;
                pointsSystem.QueryPointsCmd = data.QueryPointsCmd;
                pointsSystem.QueryPointsTips = data.QueryPointsTips;
                pointsSystem.RewardCount = data.RewardCount;
                pointsSystem.SignCmd = data.SignCmd;
                pointsSystem.SignFailTips = data.SignFailTips;
                pointsSystem.SignSucceedTips = data.SignSucceedTips;
                pointsSystem.InitialCount = data.InitialCount;
                pointsSystem.SignInterval = data.SignInterval;

                ConfigManager.Save(pointsSystem);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables_PointsSystem", "RetrieveAvailableVariables_PointsSystem", _ =>
            {
                var pointsSystem = FunctionManager.PointsSystem;

                return SucceededResult(pointsSystem.AvailableVariables);
            });

            HttpGet("/RetrievePlayerPoints", "RetrievePlayerPoints", _ =>
            {
                string steamId = Request.Query["steamId"];
                if (string.IsNullOrEmpty(steamId))
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                var data = _pointsRepository.QueryBySteamId(steamId);

                if(data == null)
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                return SucceededResult(data);
            });

            HttpPost("/UpdatePlayerPoints", "UpdatePlayerPoints", _ =>
            {
                var data = this.Bind<PointsInfoViewModel>();

                var entity = new T_Points()
                {
                    Count = data.Count,
                    LastSignDay = data.LastSignDay,
                    SteamId = data.SteamId
                };

                int count = _pointsRepository.Update(entity);

                if(count != 1)
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                return SucceededResult();
            });

            HttpGet("/IncreasePlayerPoints", "IncreasePlayerPoints", _ =>
            {
                string steamId = Request.Query["steamId"];
                if (string.IsNullOrEmpty(steamId))
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                int count = Request.Query["count"];
                if (count < 1)
                {
                    return FailedResult(message: "The count cannot be less than 1");
                }

                count = _pointsRepository.IncreasePlayerPoints(steamId, count);

                if (count != 1)
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                return SucceededResult();
            });

            HttpGet("/DeductPlayerPoints", "DeductPlayerPoints", _ =>
            {
                string steamId = Request.Query["steamId"];
                if (string.IsNullOrEmpty(steamId))
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                int count = Request.Query["count"];
                if (count < 1)
                {
                    return FailedResult(message: "The count cannot be less than 1");
                }

                bool allowNegative = Request.Query["allowNegative"];
                if (allowNegative == false)
                {
                    int ownCount = _pointsRepository.QueryPointsCountBySteamId(steamId);
                    if (ownCount < count)
                    {
                        return FailedResult(message: "Not enough points");
                    }
                }

                count = _pointsRepository.DeductPlayerPoints(steamId, count);

                if (count != 1)
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                return SucceededResult();
            });

            HttpGet("/ResetLastSignDay", "ResetLastSignDay", _ =>
            {
                string steamId = Request.Query["steamId"];

                int count = _pointsRepository.ResetLastSignDay(steamId);

                if (string.IsNullOrEmpty(steamId) == false && count != 1)
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                return SucceededResult(message: "Successfully reset " + count);
            });


            HttpPost("/RetrievePlayerPointsPaged", "RetrievePlayerPointsPaged", _ =>
            {
                var queryParams = this.Bind<PaginationQueryParams>();

                var data = _pointsRepository.QueryPaged(
                    queryParams.PageIndex,
                    queryParams.PageSize,
                    null,
                    "CreatedDate DESC",
                    null);

                return SucceededResult(data);
            });
        }
    }
}
