using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class PlayersMetadataModule : MetadataModule<PathItem>
    {
        public PlayersMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<OnlinePlayer>();
            modelCatalog.AddModel<LiveData.Inventory>();
            modelCatalog.AddModel<LiveData.InvItem>();
            modelCatalog.AddModel<Position>();
            modelCatalog.AddModel<T_Player>();

            Describe["RetrieveOnlinePlayer"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveOnlinePlayer")
                            .Tag("Players")
                            .Summary("获取在线玩家")
                            .Parameter(new Parameter() { Name = "steamId", In = ParameterIn.Query })
                            .Description("If the parameter steamId is not null then return a single player, otherwise returns a list of all online players")
                            .Response(r => r.Schema<OnlinePlayer>().Description("Online players"))));

            Describe["RetrieveKnownPlayer"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveKnownPlayer")
                            .Tag("Players")
                            .Summary("获取历史玩家")
                            .Parameter(new Parameter() { Name = "steamId", In = ParameterIn.Query })
                            .Description("If the parameter steamId is not null then return a single player, otherwise returns a list of all known players")
                            .Response(r => r.Schema<T_Player>().Description("Known player"))));

            Describe["RetrieveInventory"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveInventory")
                            .Tag("Players")
                            .Summary("获取玩家背包")
                            .Parameter(new Parameter() { Name = "steamId", In = ParameterIn.Query, Required = true })
                            .Description("If the parameter steamId is null then return a failed result, otherwise returns player's inventory")
                            .Response(r => r.Schema<LiveData.Inventory>().Description("The player's inventory"))));

        }
    }
}
