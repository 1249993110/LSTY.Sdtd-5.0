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
    public class ExtensionFunctionsMetadataModule : MetadataModule<PathItem>
    {
        public ExtensionFunctionsMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            #region DeathPenalty
            modelCatalog.AddModel<DeathPenaltyConfigViewModel>();

            Describe["RetrieveDeathPenaltyConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveDeathPenaltyConfig")
                            .Tag("ExtensionFunctions")
                            .Summary("获取死亡惩罚配置")
                            .Description("")
                            .Response(r => r.Schema<DeathPenaltyConfigViewModel>().Description("The config of DeathPenalty"))));

            Describe["UpdateDeathPenaltyConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateDeathPenaltyConfig")
                            .Tag("ExtensionFunctions")
                            .Summary("更新死亡惩罚配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(DeathPenaltyConfigViewModel)).Schema<DeathPenaltyConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_DeathPenalty"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveAvailableVariables_DeathPenalty")
                           .Tag("ExtensionFunctions")
                           .Summary("获取可用变量")
                           .Description("Get the availableVariables")
                           .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            #endregion

            #region OnlineReward
            modelCatalog.AddModel<OnlineRewardConfigViewModel>();

            Describe["RetrieveOnlineRewardConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveOnlineRewardConfig")
                            .Tag("ExtensionFunctions")
                            .Summary("获取在线奖励配置")
                            .Description("")
                            .Response(r => r.Schema<OnlineRewardConfigViewModel>().Description("The config of OnlineReward"))));

            Describe["UpdateOnlineRewardConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateOnlineRewardConfig")
                            .Tag("ExtensionFunctions")
                            .Summary("更新在线奖励配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(OnlineRewardConfigViewModel)).Schema<OnlineRewardConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_OnlineReward"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveAvailableVariables_OnlineReward")
                           .Tag("ExtensionFunctions")
                           .Summary("获取可用变量")
                           .Description("Get the availableVariables")
                           .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            #endregion

            #region AutoRestart
            modelCatalog.AddModel<AutoRestartConfigViewModel>();

            Describe["RetrieveAutoRestartConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAutoRestartConfig")
                            .Tag("ExtensionFunctions")
                            .Summary("获取自动重启配置")
                            .Description("")
                            .Response(r => r.Schema<AutoRestartConfigViewModel>().Description("The config of AutoRestart"))));

            Describe["UpdateAutoRestartConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateAutoRestartConfig")
                            .Tag("ExtensionFunctions")
                            .Summary("更新自动重启配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(AutoRestartConfigViewModel)).Schema<AutoRestartConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_AutoRestart"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveAvailableVariables_AutoRestart")
                           .Tag("ExtensionFunctions")
                           .Summary("获取可用变量")
                           .Description("Get the availableVariables")
                           .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            #endregion
        }
    }
}