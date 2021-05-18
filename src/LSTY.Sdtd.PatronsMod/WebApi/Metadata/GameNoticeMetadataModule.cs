using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Functions;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.ViewModels;
using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class GameNoticeMetadataModule : MetadataModule<PathItem>
    {
        public GameNoticeMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<GameNoticeConfigViewModel>();

            Describe["RetrieveGameNoticeConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveGameNoticeConfig")
                            .Tag("GameNotice")
                            .Summary("获取游戏公告配置")
                            .Description("")
                            .Response(r => r.Schema<GameNoticeConfigViewModel>().Description("The config of GameNotice"))));

            Describe["UpdateGameNoticeConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateGameNoticeConfig")
                            .Tag("GameNotice")
                            .Summary("更新游戏公告配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(GameNoticeConfigViewModel)).Schema<GameNoticeConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_GameNotice"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveAvailableVariables_GameNotice")
                           .Tag("GameNotice")
                           .Summary("获取可用变量")
                           .Description("Get the availableVariables")
                           .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
        }
    }
}