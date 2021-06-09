using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.Functions;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class LotterySystemMetadataModule : MetadataModule<PathItem>
    {
        public LotterySystemMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<T_Lottery>();
            modelCatalog.AddModel<IEnumerable<T_Lottery>>();
            modelCatalog.AddModel<LotterySystemConfigViewModel>();
            modelCatalog.AddModel<LotteryViewModelBase>();
            modelCatalog.AddModel<LotteryViewModel>();
            modelCatalog.AddModel<DeleteQueryParamOfString>();

            Describe["RetrieveLotterySystemConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveLotterySystemConfig")
                            .Tag("LotterySystem")
                            .Summary("获取抽奖系统配置")
                            .Description("")
                            .Response(r => r.Schema<LotterySystemConfigViewModel>().Description("The config of LotterySystem"))));

            Describe["UpdateLotterySystemConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateLotterySystemConfig")
                            .Tag("LotterySystem")
                            .Summary("更新抽奖系统配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(LotterySystemConfigViewModel)).Schema<LotterySystemConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_LotterySystem"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAvailableVariables_LotterySystem")
                            .Tag("LotterySystem")
                            .Summary("获取可用变量")
                            .Description("Get the availableVariables")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            
            Describe["CreateLottery"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("CreateLottery")
                            .Tag("LotterySystem")
                            .Summary("创建奖品")
                            .BodyParameter(p => p.Description("A object").Name(nameof(LotteryViewModelBase)).Schema<LotteryViewModelBase>())
                            .Description("")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveLottery"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveLottery")
                            .Tag("LotterySystem")
                            .Summary("获取奖品")
                            .Description("returns all goods")
                            .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<T_Lottery>>(modelCatalog).Description("Succeeded or failed"))));

            Describe["UpdateLottery"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateLottery")
                            .Tag("LotterySystem")
                            .Summary("更新奖品")
                            .Description("")
                            .BodyParameter(p => p.Description("A object").Name(nameof(LotteryViewModel)).Schema<LotteryViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["DeleteLottery"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("DeleteLottery")
                            .Tag("LotterySystem")
                            .Summary("删除奖品")
                            .BodyParameter(p => p.Description("A array").Name(nameof(DeleteQueryParamOfString)).Schema<DeleteQueryParamOfString>())
                            .Description("Delete goods by ids")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
        }
    }
}
