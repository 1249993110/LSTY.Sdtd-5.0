using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy.ModelBinding;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class ServerManageModule : ApiModuleBase
    {
        private static readonly IPlayerRepository _playerRepository;

        static ServerManageModule()
        {
            _playerRepository = IocContainer.Resolve<IPlayerRepository>();
        }

        public ServerManageModule() : base("/ServerManage")
        {
            HttpGet("/ExecuteConsoleCommand", "ExecuteConsoleCommand", _ =>
            {
                string command = Request.Query["command"];
                if (string.IsNullOrEmpty(command))
                {
                    return FailedResult("No command given");
                }

                string query = Request.Query["isAsync"];
                bool isAsync = string.IsNullOrEmpty(query) ? false : Convert.ToBoolean(query);
                List<string> executeResult = null;
                if (isAsync)
                {
                    ModHelper.MainThreadContext.Send((obj) =>
                    {
                        executeResult = SdtdConsole.Instance.ExecuteSync(command, new ClientInfo() { playerId = "LSTY.WebApi" });
                    }, null);
                }
                else
                {
                    executeResult = SdtdConsole.Instance.ExecuteSync(command, new ClientInfo() { playerId = "LSTY.WebApi" });
                }

                return SucceededResult(executeResult);
            });

            HttpGet("/RetrieveAllCommands", "RetrieveAllCommands", _ =>
            {
                List<ConsoleCommand> consoleCommands = new List<ConsoleCommand>();

                foreach (IConsoleCommand cc in SdtdConsole.Instance.GetCommands())
                {
                    consoleCommands.Add(new ConsoleCommand() 
                    {
                        Command = cc.GetCommands(),
                        Description = cc.GetDescription(),
                        Help = cc.GetHelp()
                    });
                }

                return SucceededResult(consoleCommands);
            });

            HttpGet("/RetrieveServerInfo", "RetrieveServerInfo", _ =>
            {
                if(ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                GameServerInfo gsi = ConnectionManager.Instance.LocalServerInfo;

                JObject serverInfo = new JObject();

                foreach (string stringGamePref in Enum.GetNames(typeof(GameInfoString)))
                {
                    string value = gsi.GetValue((GameInfoString)Enum.Parse(typeof(GameInfoString), stringGamePref));

                    JObject singleStat = new JObject();
                    singleStat.Add("type", "string");
                    singleStat.Add("value", value);

                    serverInfo.Add(stringGamePref, singleStat);
                }

                foreach (string intGamePref in Enum.GetNames(typeof(GameInfoInt)))
                {
                    int value = gsi.GetValue((GameInfoInt)Enum.Parse(typeof(GameInfoInt), intGamePref));

                    JObject singleStat = new JObject();
                    singleStat.Add("type", "int");
                    singleStat.Add("value", value);

                    serverInfo.Add(intGamePref, singleStat);
                }

                foreach (string boolGamePref in Enum.GetNames(typeof(GameInfoBool)))
                {
                    bool value = gsi.GetValue((GameInfoBool)Enum.Parse(typeof(GameInfoBool), boolGamePref));

                    JObject singleStat = new JObject();
                    singleStat.Add("type", "bool");
                    singleStat.Add("value", value);

                    serverInfo.Add(boolGamePref, singleStat);
                }

                return SucceededResult(serverInfo);
            });

            HttpGet("/RetrieveServerStats", "RetrieveServerStats", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                var entityList = GameManager.Instance.World.Entities.list;

                var gameStats = new Models.GameStats()
                {
                    Gametime = new Gametime()
                    {
                        Days = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime),
                        Hours = GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime),
                        Minutes = GameUtils.WorldTimeToMinutes(GameManager.Instance.World.worldTime)
                    },
                    Players = GameManager.Instance.World.Players.Count,
                    Hostiles = entityList.Count(p => p is EntityEnemy entity && entity.IsAlive()),
                    Animals = entityList.Count(p => p is EntityAnimal entity && entity.IsAlive())
                };

                return SucceededResult(gameStats);
            });

            HttpGet("/RetrieveAnimalsLocation", "RetrieveAnimalsLocation", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                var animals = GameManager.Instance.World.Entities.list.Where(p => p is EntityAnimal entity && entity.IsAlive());

                List<EntityLocation> entityLocations = new List<EntityLocation>();
                foreach (EntityAlive entity in animals)
                {
                    entityLocations.Add(new EntityLocation() 
                    {
                        Id = entity.entityId,
                        Name = entity.EntityName ?? ("animal class #" + entity.entityClass),
                        Position = new Position(entity.GetPosition())
                    });
                }

                return SucceededResult(entityLocations);
            });

            HttpGet("/RetrieveHostileLocation", "RetrieveHostileLocation", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                var enemies = GameManager.Instance.World.Entities.list.Where(p => p is EntityEnemy entity && entity.IsAlive());

                List<EntityLocation> entityLocations = new List<EntityLocation>();
                foreach (EntityAlive entity in enemies)
                {
                    entityLocations.Add(new EntityLocation()
                    {
                        Id = entity.entityId,
                        Name = entity.EntityName ?? ("enemy class #" + entity.entityClass),
                        Position = new Position(entity.GetPosition())
                    });
                }

                return SucceededResult(entityLocations);
            });

            HttpGet("/RetrievePlayersLocation", "RetrievePlayersLocation", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                string query = Request.Query["offline"];
                bool offline = string.IsNullOrEmpty(query) ? false : Convert.ToBoolean(query);

                List<PlayersLocation> playersLocations = new List<PlayersLocation>();

                if (offline)
                {
                    var players = _playerRepository.QueryAll();
                    foreach (var player in players)
                    {
                        playersLocations.Add(new PlayersLocation()
                        {
                            SteamId = player.SteamId,
                            EntityId = player.EntityId,
                            Name = player.Name,
                            Online = LiveDataContainer.OnlinePlayers.ContainsKey(player.SteamId),
                            Position = new Position(player.LastPositionX, player.LastPositionY, player.LastPositionZ)
                        });
                    }
                }
                else
                {
                    foreach (var player in LiveDataContainer.OnlinePlayers.Values)
                    {
                        playersLocations.Add(new PlayersLocation() 
                        {
                            SteamId = player.SteamId,
                            EntityId = player.EntityId,
                            Name = player.Name,
                            Online = true,
                            Position = player.LastPosition
                        });
                    }
                }

                return SucceededResult(playersLocations);
            });

            HttpGet("/RetrieveLandClaims", "RetrieveLandClaims", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                string steamIdFilter = Request.Query["steamId"];
                bool filter = string.IsNullOrEmpty(steamIdFilter) == false;

                var persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
                var pBlockMap = persistentPlayerList.m_lpBlockMap;

                Dictionary<string, ClaimOwner> claimOwners = new Dictionary<string, ClaimOwner>();

                var playerNameDict = _playerRepository.QueryNameDict();

                string steamId = null;
                string playerName = null;
                foreach (var item in pBlockMap)
                {
                    steamId = item.Value.PlayerId;

                    if(filter && steamId != steamIdFilter)
                    {
                        continue;
                    }

                    if(playerNameDict.TryGetValue(steamId, out playerName) == false)
                    {
                        if(LiveDataContainer.OnlinePlayers.TryGetValue(steamId, out var onlinePlayer))
                        {
                            playerName = onlinePlayer.Name;
                        }
                    }

                    claimOwners.TryAdd(steamId, new ClaimOwner() 
                    {
                        Claimactive = GameManager.Instance.World.IsLandProtectionValidForPlayer(persistentPlayerList.GetPlayerData(steamId)),
                        SteamId = steamId,
                        EntityId = item.Value.EntityId,
                        PlayerName = playerName,
                        Claims = new List<Position>()
                    });

                    claimOwners[steamId].Claims.Add(new Position(item.Key));
                }

                LandClaims landClaims = new LandClaims() 
                {
                    ClaimOwners = claimOwners.Values.ToList(),
                    Claimsize = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("LandClaimSize"))
                };

                return SucceededResult(landClaims);
            });

            HttpGet("/RestartServer", "RestartServer", _ =>
            {
                string query = Request.Query["force"];
                bool force = string.IsNullOrEmpty(query) ? false : Convert.ToBoolean(query);

                if (force)
                {
                    ModHelper.RestartServer(true);
                }
                else
                {
                    if (ModHelper.GameStartDone == false)
                    {
                        return FailedResult("Server is starting, please use force parameter");
                    }
                    else
                    {
                        ModHelper.RestartServer(false);
                    }
                }

                return SucceededResult();
            });

            HttpGet("/RetrieveAllAvailableItems", "RetrieveAllAvailableItems", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                return SucceededResult(GetGameItemBlocks(false));
            });

            HttpPost("/RetrieveAvailableItemsPaged", "RetrieveAvailableItemsPaged", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                var queryParams = this.Bind<PaginationQueryParams>();

                var data = GetGameItemBlocks(false);

                PaginationQueryResult paginationQueryResult = new PaginationQueryResult()
                {
                    Items = data.Skip((queryParams.PageIndex - 1) * queryParams.PageSize).Take(queryParams.PageSize),
                    Total = data.Count
                };

                return SucceededResult(paginationQueryResult);
            });

            HttpGet("/RetrieveAllAvailableBlocks", "RetrieveAllAvailableBlocks", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                return SucceededResult(GetGameItemBlocks(true));
            });

            HttpPost("/RetrieveAvailableBlocksPaged", "RetrieveAvailableBlocksPaged", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                var queryParams = this.Bind<PaginationQueryParams>();

                var data = GetGameItemBlocks(true);

                PaginationQueryResult paginationQueryResult = new PaginationQueryResult()
                {
                    Items = data.Skip((queryParams.PageIndex - 1) * queryParams.PageSize).Take(queryParams.PageSize),
                    Total = data.Count
                };

                return SucceededResult(paginationQueryResult);
            });

            HttpGet("/RetrieveAllAvailableEntities", "RetrieveAllAvailableEntities", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                return SucceededResult(GetGameEntities());
            });

            HttpPost("/RetrieveAvailableEntitiesPaged", "RetrieveAvailableEntitiesPaged", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult("Server is starting, please wait");
                }

                var queryParams = this.Bind<PaginationQueryParams>();

                var data = GetGameEntities();

                PaginationQueryResult paginationQueryResult = new PaginationQueryResult()
                {
                    Items = data.Skip((queryParams.PageIndex - 1) * queryParams.PageSize).Take(queryParams.PageSize),
                    Total = data.Count
                };

                return SucceededResult(paginationQueryResult);
            });
        }

        private static List<GameItemBlocks> GetGameItemBlocks(bool getBlock)
        {
            if (ItemClass.list == null)
            {
                // 确保已运行指定类型的类型初始值设定项（也称为静态构造函数）。
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(WorldStaticData).TypeHandle);
            }

            List<GameItemBlocks> data = new List<GameItemBlocks>();

            if (getBlock)
            {
                foreach (var item in ItemClass.list)
                {
                    if (item != null && item.IsBlock())
                    {
                        data.Add(new GameItemBlocks()
                        {
                            Id = item.Id,
                            Name = item.Name
                        });
                    }
                }
            }
            else
            {
                foreach (var item in ItemClass.list)
                {
                    if (item != null && item.IsBlock() == false)
                    {
                        data.Add(new GameItemBlocks()
                        {
                            Id = item.Id,
                            Name = item.Name
                        });
                    }
                }
            }

            return data;
        }

        private static List<GameEntities> GetGameEntities()
        {
            int num = 1;
            List<GameEntities> data = new List<GameEntities>();
            foreach (var item in EntityClass.list.Dict.Values)
            {
                if (item.bAllowUserInstantiate)
                {
                    data.Add(new GameEntities()
                    {
                        Id = num,
                        Name = item.entityClassName
                    });

                    ++num;
                }
            }

            return data;
        }
    }
}
