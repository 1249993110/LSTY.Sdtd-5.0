using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy.ModelBinding;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class ServerManageModule : ApiModuleBase
    {
        public ServerManageModule()
        {
            HttpGet("/ExecuteConsoleCommand", "ExecuteConsoleCommand", _ =>
            {
                string command = Request.Query["command"];
                if (string.IsNullOrEmpty(command))
                {
                    return FailedResult("No command given");
                }

                List<string> executeResult = SdtdConsole.Instance.ExecuteSync(command, null);

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
                    Hostiles = entityList.Count(p => p is EntityEnemy entity && entity != null && entity.IsAlive()),
                    Animals = entityList.Count(p => p is EntityAnimal entity && entity != null && entity.IsAlive())
                };

                return SucceededResult(gameStats);
            });
        }
    }
}
