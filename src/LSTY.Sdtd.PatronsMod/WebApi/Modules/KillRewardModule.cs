using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy.ModelBinding;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class KillRewardModule : ApiModuleBase
    {
        private static readonly IKillRewardRepository _killRewardRepository = IocContainer.Resolve<IKillRewardRepository>();

        public KillRewardModule() : base("/KillReward")
        {
            HttpGet("/RetrieveKillRewardConfig", "RetrieveKillRewardConfig", _ =>
            {
                var function = FunctionManager.KillReward;
                var data = new KillRewardConfigViewModel()
                {
                    FunctionName = function.FunctionName,
                    IsEnabled = function.IsEnabled
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateKillRewardConfig", "UpdateKillRewardConfig", _ =>
            {
                var data = this.Bind<KillRewardConfigViewModel>();
                var function = FunctionManager.KillReward;

                function.IsEnabled = data.IsEnabled;

                ConfigManager.Save(function);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables", "RetrieveAvailableVariables_KillReward", _ =>
            {
                return SucceededResult(FunctionManager.KillReward.AvailableVariables);
            });

            HttpPost("/CreateKillReward", "CreateKillReward", _ =>
            {
                var data = this.Bind<KillRewardViewModel>();
                _killRewardRepository.Insert(new T_KillReward()
                {
                    Id = Guid.NewGuid().ToString(),
                    FriendlyName = data.FriendlyName,
                    KilledTips = data.KilledTips,
                    RewardContent = data.RewardContent,
                    RewardContentType = data.RewardContentType,
                    RewardCount = data.RewardCount,
                    RewardQuality = data.RewardQuality,
                    SpawnedTips = data.SpawnedTips,
                    SteamIdOrEntityName = data.SteamIdOrEntityName
                });

                return SucceededResult();
            });

            HttpGet("/RetrieveKillReward", "RetrieveKillReward", _ =>
            {
                string id = Request.Query["killRewardId"];
                if (string.IsNullOrEmpty(id))
                {
                    var entitys = _killRewardRepository.QueryAll();
                    return SucceededResult(entitys);
                }

                var data = _killRewardRepository.QueryById(nameof(T_KillReward.Id), id);

                if (data == null || data.Any() == false)
                {
                    return FailedResult(message: "The specified killRewardId does not exist");
                }

                return SucceededResult(data);
            });

            HttpPost("/UpdateKillReward", "UpdateKillReward", _ =>
            {
                var data = this.Bind<KillRewardViewModel>();

                var entity = new T_KillReward()
                {
                    Id = data.Id,
                    FriendlyName = data.FriendlyName,
                    KilledTips = data.KilledTips,
                    RewardContent = data.RewardContent,
                    RewardContentType = data.RewardContentType,
                    RewardCount = data.RewardCount,
                    RewardQuality = data.RewardQuality,
                    SpawnedTips = data.SpawnedTips,
                    SteamIdOrEntityName = data.SteamIdOrEntityName
                };

                int count = _killRewardRepository.Update(entity);

                if (count != 1)
                {
                    return FailedResult(message: "The specified killRewardId does not exist");
                }

                return SucceededResult();
            });

            HttpPost("/DeleteKillReward", "DeleteKillReward", _ =>
            {
                var data = this.Bind<DeleteQueryParamOfString>();

                if (data == null || data.Ids == null || data.Ids.Any() == false)
                {
                    return FailedResult(message: "The specified killRewardIds is null");
                }

                int count = _killRewardRepository.DeleteBatchByIds(nameof(T_KillReward.Id), data.Ids);

                if (count != data.Ids.Length)
                {
                    return FailedResult(message: "Deleted count: " + count);
                }

                return SucceededResult();
            });
        }
    }
}
