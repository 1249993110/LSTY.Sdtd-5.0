using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    /// <summary>
    /// The class name must end with MetadataModule
    /// </summary>
    public class ServerManageMetadataModule : MetadataModule<PathItem>
    {
        public ServerManageMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModels(
                typeof(ConsoleCommand), 
                typeof(IEnumerable<ConsoleCommand>),
                typeof(Models.GameStats), 
                typeof(Gametime),
                typeof(EntityLocation),
                typeof(IEnumerable<EntityLocation>),
                typeof(PlayersLocation),
                typeof(IEnumerable<PlayersLocation>),
                typeof(ClaimOwner),
                typeof(LandClaims));

            Describe["ExecuteConsoleCommand"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("ExecuteConsoleCommand")
                            .Tag("ServerManage")
                            .Summary("执行控制台命令")
                            .Description("")
                            .Parameters(new Parameter[]
                            {
                                new Parameter() { Name = "command", In = ParameterIn.Query, Required = true },
                                new Parameter() { Name = "isAsync", In = ParameterIn.Query, Description = "Whether async execute command", Required = false, Default = false }
                            })
                            .Response(r => r.Schema<IEnumerable<string>>(modelCatalog).Description("The command executed result"))));

            Describe["RetrieveAllCommands"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveAllCommands")
                           .Tag("ServerManage")
                           .Summary("获取所有控制台命令")
                           .Description("")
                           .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<ConsoleCommand>>(modelCatalog).Description("A list of all console command"))));

            Describe["RetrieveServerInfo"] = description => description.AsSwagger(
              with => with.Operation(
                  op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                          .OperationId("RetrieveServerInfo")
                          .Tag("ServerManage")
                          .Summary("获取服务器信息")
                          .Description("")
                          .Response((int)HttpStatusCode.OK, r => r.Description("The server' information"))));

            Describe["RetrieveServerStats"] = description => description.AsSwagger(
              with => with.Operation(
                  op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                          .OperationId("RetrieveServerStats")
                          .Tag("ServerManage")
                          .Summary("获取服务器统计")
                          .Description("")
                          .Response((int)HttpStatusCode.OK, r => r.Schema<GameStats>().Description("The server' stats"))));

            Describe["RetrieveAnimalsLocation"] = description => description.AsSwagger(
              with => with.Operation(
                  op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                          .OperationId("RetrieveAnimalsLocation")
                          .Tag("ServerManage")
                          .Summary("获取动物位置")
                          .Description("")
                          .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<EntityLocation>>(modelCatalog).Description("Animals location"))));

            Describe["RetrieveHostileLocation"] = description => description.AsSwagger(
              with => with.Operation(
                  op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                          .OperationId("RetrieveHostileLocation")
                          .Tag("ServerManage")
                          .Summary("获取僵尸位置")
                          .Description("")
                          .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<EntityLocation>>(modelCatalog).Description("Hostile location"))));

            Describe["RetrievePlayersLocation"] = description => description.AsSwagger(
              with => with.Operation(
                  op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                          .OperationId("RetrievePlayersLocation")
                          .Tag("ServerManage")
                          .Summary("获取玩家位置")
                          .Parameter(new Parameter() { Name = "offline", In = ParameterIn.Query, Default = false })
                          .Description("")
                          .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<PlayersLocation>>(modelCatalog).Description("Players location"))));

            Describe["RetrieveLandClaims"] = description => description.AsSwagger(
              with => with.Operation(
                  op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                          .OperationId("RetrieveLandClaims")
                          .Tag("ServerManage")
                          .Summary("获取领地石位置")
                          .Parameter(new Parameter() { Name = "steamId", In = ParameterIn.Query, Default = string.Empty })
                          .Description("")
                          .Response((int)HttpStatusCode.OK, r => r.Schema<LandClaims>(modelCatalog).Description("Land claims location"))));
        }
    }
}
