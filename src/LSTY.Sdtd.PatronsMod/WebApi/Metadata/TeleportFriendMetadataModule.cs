using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Functions;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class TeleportFriendMetadataModule : MetadataModule<PathItem>
    {
        public TeleportFriendMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<TeleportFriendConfigViewModel>();

            Describe["RetrieveTeleportFriendConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveTeleportFriendConfig")
                            .Tag("TeleportFriend")
                            .Summary("获取好友传送配置")
                            .Description("")
                            .Response(r => r.Schema<TeleportFriendConfigViewModel>().Description("The config of TeleportFriend"))));

            Describe["UpdateTeleportFriendConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateTeleportFriendConfig")
                            .Tag("TeleportFriend")
                            .Summary("更新好友传送配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(TeleportFriendConfigViewModel)).Schema<TeleportFriendConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_TeleportFriend"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveAvailableVariables_TeleportFriend")
                           .Tag("TeleportFriend")
                           .Summary("获取可用变量")
                           .Description("Get the availableVariables")
                           .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
        }
    }
}