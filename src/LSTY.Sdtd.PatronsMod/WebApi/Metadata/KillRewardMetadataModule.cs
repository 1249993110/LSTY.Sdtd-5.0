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
    public class KillRewardMetadataModule : MetadataModule<PathItem>
    {
        public KillRewardMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<T_KillReward>();
            modelCatalog.AddModel<KillRewardConfigViewModel>();
            modelCatalog.AddModel<KillRewardViewModelBase>();
            modelCatalog.AddModel<KillRewardViewModel>();
            modelCatalog.AddModel<DeleteQueryParamOfString>();

            Describe["RetrieveKillRewardConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveKillRewardConfig")
                            .Tag("KillReward")
                            .Summary("获取击杀悬赏配置")
                            .Description("")
                            .Response(r => r.Schema<KillRewardConfigViewModel>().Description("The config of KillReward"))));

            Describe["UpdateKillRewardConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateKillRewardConfig")
                            .Tag("KillReward")
                            .Summary("更新击杀悬赏配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(KillRewardConfigViewModel)).Schema<KillRewardConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_KillReward"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAvailableVariables_KillReward")
                            .Tag("KillReward")
                            .Summary("获取可用变量")
                            .Description("Get the availableVariables")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            
            Describe["CreateKillReward"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("CreateKillReward")
                            .Tag("KillReward")
                            .Summary("创建击杀悬赏项")
                            .BodyParameter(p => p.Description("A object").Name(nameof(KillRewardViewModelBase)).Schema<KillRewardViewModelBase>())
                            .Description("")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveKillReward"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveKillReward")
                            .Tag("KillReward")
                            .Summary("获取击杀悬赏项")
                            .Parameter(new Parameter() { Name = "killRewardId", In = ParameterIn.Query })
                            .Description("If the parameter killRewardId is null then return all items, otherwise returns the specified item")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["UpdateKillReward"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateKillReward")
                            .Tag("KillReward")
                            .Summary("更新击杀悬赏项")
                            .Description("")
                            .BodyParameter(p => p.Description("A object").Name(nameof(KillRewardViewModel)).Schema<KillRewardViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["DeleteKillReward"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("DeleteKillReward")
                            .Tag("KillReward")
                            .Summary("删除击杀悬赏项")
                            .BodyParameter(p => p.Description("A array").Name(nameof(DeleteQueryParamOfString)).Schema<DeleteQueryParamOfString>())
                            .Description("Delete item by ids")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

        }
    }
}
