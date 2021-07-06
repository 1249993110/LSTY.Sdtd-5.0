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
    public class CDKeyExchangeModule : ApiModuleBase
    {
        private readonly ICDKeyRepository _cdKeyRepository = IocContainer.Resolve<ICDKeyRepository>();
        private readonly ICDKeyExchangeLogRepository _cdKeyExchangeLogRepository = IocContainer.Resolve<ICDKeyExchangeLogRepository>();
        public CDKeyExchangeModule() : base("/CDKeyExchange")
        {
            HttpGet("/RetrieveCDKeyExchangeConfig", "RetrieveCDKeyExchangeConfig", _ =>
            {
                var function = FunctionManager.CDKeyExchange;
                var data = new CDKeyExchangeConfigViewModel()
                {
                    FunctionName = function.FunctionName,
                    IsEnabled = function.IsEnabled,
                    ExchangeSuccessfullyTips = function.ExchangeSuccessfullyTips,
                    InvalidKeyTips = function.InvalidKeyTips
                };

                return SucceededResult(data);
            });

            HttpPost("/UpdateCDKeyExchangeConfig", "UpdateCDKeyExchangeConfig", _ =>
            {
                var data = this.Bind<CDKeyExchangeConfigViewModel>();
                var function = FunctionManager.CDKeyExchange;

                function.IsEnabled = data.IsEnabled;
                function.ExchangeSuccessfullyTips = data.ExchangeSuccessfullyTips;
                function.InvalidKeyTips = data.InvalidKeyTips;

                ConfigManager.Save(function);

                return SucceededResult();
            });

            HttpGet("/RetrieveAvailableVariables", "RetrieveAvailableVariables_CDKeyExchange", _ =>
            {
                return SucceededResult(FunctionManager.CDKeyExchange.AvailableVariables);
            });

            HttpPost("/CreateCDKey", "CreateCDKey", _ =>
            {
                string query = Request.Query["isBatch"];
                bool isBatch = string.IsNullOrEmpty(query) ? false : Convert.ToBoolean(query);
                IEnumerable<CDKeyExchangeViewModel> data;
                if (isBatch)
                {
                    data = this.Bind<IEnumerable<CDKeyExchangeViewModel>>();
                }
                else
                {
                    data = new CDKeyExchangeViewModel[] { this.Bind<CDKeyExchangeViewModel>() };
                }

                var entities = new List<T_CDKey>();

                foreach (var item in data)
                {
                    entities.Add(new T_CDKey()
                    {
                        ExpiryDate = item.ExpiryDate,
                        ContentType = item.ContentType,
                        ItemContent = item.ItemContent,
                        ItemCount = item.ItemCount,
                        ItemName = item.ItemName,
                        ItemQuality = item.ItemQuality,
                        Key = item.Key,
                        LimitUseOnceEachPlayer = item.LimitUseOnceEachPlayer ? 1 : 0,
                        MaxExchangeCount = item.MaxExchangeCount
                    });
                }

                _cdKeyRepository.InsertBatch(entities);

                return SucceededResult();
            });

            HttpPost("/RetrieveCDKeyPaged", "RetrieveCDKeyPaged", _ =>
            {
                var queryParams = this.Bind<PaginationQueryParams>();

                var entities = _cdKeyRepository.QueryPaged(
                    queryParams.PageIndex,
                    queryParams.PageSize);

                var data = new List<T_CDKeyViewModel>();

                foreach (var item in entities)
                {
                    data.Add(new T_CDKeyViewModel()
                    {
                        Id = item.Id,
                        CreatedDate = item.CreatedDate,
                        ExpiryDate = item.ExpiryDate,
                        ContentType = item.ContentType,
                        ItemContent = item.ItemContent,
                        ItemCount = item.ItemCount,
                        ItemName = item.ItemName,
                        ItemQuality = item.ItemQuality,
                        Key = item.Key,
                        LimitUseOnceEachPlayer = item.LimitUseOnceEachPlayer == 1,
                        MaxExchangeCount = item.MaxExchangeCount
                    });
                }

                PaginationQueryResult paginationQueryResult = new PaginationQueryResult()
                {
                    Items = data,
                    Total = _cdKeyRepository.QueryRecordCount()
                };
                return SucceededResult(paginationQueryResult);
            });

            HttpPost("/UpdateCDKey", "UpdateCDKey", _ =>
            {
                var data = this.Bind<CDKeyExchangeViewModel>();

                var entity = new T_CDKey()
                {
                    Id = data.Id,
                    ExpiryDate = data.ExpiryDate,
                    ContentType = data.ContentType,
                    ItemContent = data.ItemContent,
                    ItemCount = data.ItemCount,
                    ItemName = data.ItemName,
                    ItemQuality = data.ItemQuality,
                    Key = data.Key,
                    LimitUseOnceEachPlayer = data.LimitUseOnceEachPlayer ? 1 : 0,
                    MaxExchangeCount = data.MaxExchangeCount
                };

                int count = _cdKeyRepository.Update(entity);

                if (count != 1)
                {
                    return FailedResult(message: "The specified cdKeyExchangeId does not exist");
                }

                return SucceededResult();
            });

            HttpPost("/DeleteCDKey", "DeleteCDKey", _ =>
            {
                var data = this.Bind<DeleteQueryParamOfString>();

                if (data == null || data.Ids == null || data.Ids.Any() == false)
                {
                    return FailedResult(message: "The specified cdKeyExchangeIds is null");
                }

                int count = _cdKeyRepository.DeleteBatchByIds(nameof(T_CDKey.Id), data.Ids);

                if (count != data.Ids.Length)
                {
                    return FailedResult(message: "Deleted count: " + count);
                }

                return SucceededResult();
            });

            HttpPost("/RetrieveCDKeyExchangeLogPaged", "RetrieveCDKeyExchangeLogPaged", _ =>
            {
                var queryParams = this.Bind<PaginationQueryParams>();

                var entities = _cdKeyExchangeLogRepository.QueryPaged(
                    queryParams.PageIndex,
                    queryParams.PageSize);

                PaginationQueryResult paginationQueryResult = new PaginationQueryResult()
                {
                    Items = entities,
                    Total = _cdKeyExchangeLogRepository.QueryRecordCount()
                };

                return SucceededResult(paginationQueryResult);
            });
        }
    }
}
