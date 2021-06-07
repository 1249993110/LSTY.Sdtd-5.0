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
    public class GameStoreModule : ApiModuleBase
    {
        private static readonly IGoodsRepository _goodsRepository;

        static GameStoreModule()
        {
            _goodsRepository = IocContainer.Resolve<IGoodsRepository>();
        }

        public GameStoreModule()
        {
            HttpGet("/RetrieveGameStoreConfig", "RetrieveGameStoreConfig", _ =>
            {
                var function = FunctionManager.GameStore;
                var data = new GameStoreConfigViewModel()
                {
                    QueryListPreTips = function.QueryListPreTips,
                    FunctionName = function.FunctionName,
                    IsEnabled = function.IsEnabled,
                    QueryListCmd = function.QueryListCmd,
                    QueryListTips = function.QueryListTips,
                    PointsNotEnoughTips = function.PointsNotEnoughTips,
                    BuySuccessfullyTips = function.BuySuccessfullyTips,
                    GoodsNoFoundTips = function.GoodsNoFoundTips
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateGameStoreConfig", "UpdateGameStoreConfig", _ =>
            {
                var data = this.Bind<GameStoreConfigViewModel>();
                var function = FunctionManager.GameStore;

                function.QueryListPreTips = data.QueryListPreTips;
                function.IsEnabled = data.IsEnabled;
                function.QueryListCmd = data.QueryListCmd;
                function.QueryListTips = data.QueryListTips;
                function.PointsNotEnoughTips = data.PointsNotEnoughTips;
                function.BuySuccessfullyTips = data.BuySuccessfullyTips;
                function.GoodsNoFoundTips = data.GoodsNoFoundTips;

                ConfigManager.Save(function);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables_GameStore", "RetrieveAvailableVariables_GameStore", _ =>
            {
                return SucceededResult(FunctionManager.GameStore.AvailableVariables);
            });

            HttpPost("/CreateGoods", "CreateGoods", _ =>
            {
                var data = this.Bind<GoodsViewModel>();
                _goodsRepository.Insert(new T_Goods() 
                {
                    Id = Guid.NewGuid().ToString(),
                    BuyCmd = data.BuyCmd,
                    Content = data.Content,
                    ContentType = data.ContentType,
                    Count = data.Count,
                    Name = data.Name,
                    Price = data.Price,
                    Quality = data.Quality
                });

                return SucceededResult();
            });

            HttpGet("/RetrieveGoods", "RetrieveGoods", _ =>
            {
                return SucceededResult(_goodsRepository.QueryAll());
            });

            HttpPost("/UpdateGoods", "UpdateGoods", _ =>
            {
                var data = this.Bind<GoodsViewModel>();

                var entity = new T_Goods()
                {
                    Id = data.Id,
                    BuyCmd = data.BuyCmd,
                    Content = data.Content,
                    ContentType = data.ContentType,
                    Count = data.Count,
                    Name = data.Name,
                    Price = data.Price,
                    Quality = data.Quality
                };

                int count = _goodsRepository.Update(entity);

                if (count != 1)
                {
                    return FailedResult(message: "The specified goodsId does not exist");
                }

                return SucceededResult();
            });

            HttpPost("/DeleteGoods", "DeleteGoods", _ =>
            {
                var data = this.Bind<DeleteQueryParamOfString>();

                if (data == null || data.Ids == null || data.Ids.Any() == false)
                {
                    return FailedResult(message: "The specified goodsIds is null");
                }

                int count = _goodsRepository.DeleteBatchByIds(nameof(T_Goods.Id), data.Ids);

                if (count != data.Ids.Length)
                {
                    return FailedResult(message: "Deleted count: " + count);
                }
                
                return SucceededResult();
            });

            HttpGet("/RetrieveContentTypes", "RetrieveContentTypes", _ =>
            {
                return SucceededResult(IocContainer.Resolve<IContentTypesRepository>().QueryAll());
            });
        }
    }
}
