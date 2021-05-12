using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Functions;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.ViewModels;
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
                            .Response(200, r => r.Description("Succeeded or failed"))));

        }
    }
}