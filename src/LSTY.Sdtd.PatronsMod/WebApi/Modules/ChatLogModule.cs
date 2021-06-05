using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy.ModelBinding;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class ChatLogModule : ApiModuleBase
    {
        private static readonly IVChatLogRepository _vChatLogRepository = IocContainer.Resolve<IVChatLogRepository>();

        public ChatLogModule()
        {
            HttpGet("/RetrieveChatLogBySteamId", "RetrieveChatLogBySteamId", _ =>
            {
                string steamId = Request.Query["steamId"];
                if (string.IsNullOrEmpty(steamId))
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                var data = _vChatLogRepository.QueryBySteamId(steamId);

                return SucceededResult(data);
            });

            HttpGet("/RetrieveChatLogByEntityId", "RetrieveChatLogByEntityId", _ =>
            {
                string entityIdStr = Request.Query["entityId"];
                if (string.IsNullOrEmpty(entityIdStr) || int.TryParse(entityIdStr,out int entityId) == false)
                {
                    return FailedResult(message: "The specified entityId does not exist");
                }

                var data = _vChatLogRepository.QueryByEntityId(entityId);

                return SucceededResult(data);
            });

            HttpGet("/RetrieveChatLogByDateTime", "RetrieveChatLogByDateTime", _ =>
            {
                string startDateTimeStr = Request.Query["startDateTime"];
                string endDateTimeStr = Request.Query["endDateTime"];

                if (string.IsNullOrEmpty(startDateTimeStr)
                    || string.IsNullOrEmpty(endDateTimeStr))
                {
                    return FailedResult(message: "The specified query parameters does not exist");
                }

                var data = _vChatLogRepository.QueryByDateTime(Convert.ToDateTime(startDateTimeStr), Convert.ToDateTime(endDateTimeStr), "CreatedDate DESC");

                return SucceededResult(data);
            });

            HttpPost("/RetrieveChatLogPaged", "RetrieveChatLogPaged", _ =>
            {
                var queryParams = this.Bind<ChatLogQueryParams>();

                bool steamIdExist = string.IsNullOrEmpty(queryParams.SteamId);

                string whereBy = steamIdExist ? null : "SteamId=@SteamId";
              
                object param = steamIdExist ? null : new { SteamId = queryParams.SteamId };

                var data = _vChatLogRepository.QueryPaged(
                    queryParams.PageIndex,
                    queryParams.PageSize,
                    whereBy,
                    "CreatedDate DESC",
                    param);

                return SucceededResult(data);
            });
        }
    }
}
