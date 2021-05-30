using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.LiveData;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class PlayersModule : ApiModuleBase
    {
        private static readonly IPlayerRepository _playerRepository;
        private static readonly IInventoryRepository _inventoryRepository;

        static PlayersModule()
        {
            _playerRepository = IocContainer.Resolve<IPlayerRepository>();
            _inventoryRepository = IocContainer.Resolve<IInventoryRepository>();
        }

        public PlayersModule()
        {
            HttpGet("/RetrieveOnlinePlayer", "RetrieveOnlinePlayer", _ =>
            {
                string steamId = Request.Query["steamId"];
                if (string.IsNullOrEmpty(steamId) == false)
                {
                    if (LiveDataContainer.OnlinePlayers.TryGetValue(steamId, out var player) == false)
                    {
                        return FailedResult(message: "The specified steamId does not exist");
                    }

                    return SucceededResult(player);
                }

                return SucceededResult(LiveDataContainer.OnlinePlayers);
            });

            HttpGet("/RetrieveKnownPlayer", "RetrieveKnownPlayer", _ =>
            {
                string steamId = Request.Query["steamId"];
                if (string.IsNullOrEmpty(steamId) == false)
                {
                    T_Player player = _playerRepository.QueryBySteamId(steamId);
                    if (player == null)
                    {
                        return FailedResult(message: "The specified steamId does not exist");
                    }

                    return SucceededResult(player);
                }

                return SucceededResult(_playerRepository.QueryAll());
            });

            HttpGet("/RetrieveInventory", "RetrieveInventory", _ =>
            {
                string steamId = Request.Query["steamId"];
                if (string.IsNullOrEmpty(steamId))
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                if (LiveDataContainer.OnlinePlayers.TryGetValue(steamId, out var player))
                {
                    return SucceededResult(player.GetInventory());
                }

                var inventory = _inventoryRepository.QueryBySteamId(steamId);
                if (inventory == null)
                {
                    return FailedResult(message: "The specified steamId does not exist");
                }

                return SucceededResult(JsonConvert.DeserializeObject(inventory.Content));
            });
        }
    }
}