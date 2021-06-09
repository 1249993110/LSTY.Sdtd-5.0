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
    public class LotterySystemModule : ApiModuleBase
    {
        private static readonly ILotteryRepository _lotteryRepository;

        static LotterySystemModule()
        {
            _lotteryRepository = IocContainer.Resolve<ILotteryRepository>();
        }

        public LotterySystemModule()
        {
            HttpGet("/RetrieveLotterySystemConfig", "RetrieveLotterySystemConfig", _ =>
            {
                var function = FunctionManager.LotterySystem;
                var data = new LotterySystemConfigViewModel()
                {
                    FunctionName = function.FunctionName,
                    IsEnabled = function.IsEnabled,
                    CurrentLotteryCmd = function.CurrentLotteryCmd,
                    EndLotteryTips = function.EndLotteryTips,
                    LotteryDuration = function.LotteryDuration,
                    LotteryInterval = function.LotteryInterval,
                    MaxWinnerCount = function.MaxWinnerCount,
                    NotWinningTips = function.NotWinningTips,
                    StartLotteryTips = function.StartLotteryTips,
                    WinningTips = function.WinningTips
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateLotterySystemConfig", "UpdateLotterySystemConfig", _ =>
            {
                var data = this.Bind<LotterySystemConfigViewModel>();
                var function = FunctionManager.LotterySystem;

                function.IsEnabled = data.IsEnabled;
                function.CurrentLotteryCmd = data.CurrentLotteryCmd;
                function.EndLotteryTips = data.EndLotteryTips;
                function.LotteryDuration = data.LotteryDuration;
                function.LotteryInterval = data.LotteryInterval;
                function.MaxWinnerCount = data.MaxWinnerCount;
                function.NotWinningTips = data.NotWinningTips;
                function.StartLotteryTips = data.StartLotteryTips;
                function.WinningTips = data.WinningTips;

                ConfigManager.Save(function);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables_LotterySystem", "RetrieveAvailableVariables_LotterySystem", _ =>
            {
                return SucceededResult(FunctionManager.LotterySystem.AvailableVariables);
            });

            HttpPost("/CreateLottery", "CreateLottery", _ =>
            {
                var data = this.Bind<LotteryViewModel>();
                _lotteryRepository.Insert(new T_Lottery() 
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = data.Content,
                    ContentType = data.ContentType,
                    Count = data.Count,
                    Name = data.Name,
                    Quality = data.Quality
                });

                return SucceededResult();
            });

            HttpGet("/RetrieveLottery", "RetrieveLottery", _ =>
            {
                return SucceededResult(_lotteryRepository.QueryAll());
            });

            HttpPost("/UpdateLottery", "UpdateLottery", _ =>
            {
                var data = this.Bind<LotteryViewModel>();

                var entity = new T_Lottery()
                {
                    Id = data.Id,
                    Content = data.Content,
                    ContentType = data.ContentType,
                    Count = data.Count,
                    Name = data.Name,
                    Quality = data.Quality
                };

                int count = _lotteryRepository.Update(entity);

                if (count != 1)
                {
                    return FailedResult(message: "The specified lotteryId does not exist");
                }

                return SucceededResult();
            });

            HttpPost("/DeleteLottery", "DeleteLottery", _ =>
            {
                var data = this.Bind<DeleteQueryParamOfString>();

                if (data == null || data.Ids == null || data.Ids.Any() == false)
                {
                    return FailedResult(message: "The specified lotteryIds is null");
                }

                int count = _lotteryRepository.DeleteBatchByIds(nameof(T_Lottery.Id), data.Ids);

                if (count != data.Ids.Length)
                {
                    return FailedResult(message: "Deleted count: " + count);
                }
                
                return SucceededResult();
            });
        }
    }
}
