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
    public class PointsSystemMetadataModule : MetadataModule<PathItem>
    {
        public PointsSystemMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<T_Points>();
            modelCatalog.AddModel<PointsSystemConfigViewModel>();
            modelCatalog.AddModel<PointsInfoViewModel>();

            Describe["RetrievePointsSystemConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrievePointsSystemConfig")
                            .Tag("PointsSystem")
                            .Summary("获取积分系统配置")
                            .Description("")
                            .Response(r => r.Schema<PointsSystemConfigViewModel>().Description("The config of PointsSystem"))));

            Describe["UpdatePointsSystemConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdatePointsSystemConfig")
                            .Tag("PointsSystem")
                            .Summary("更新积分系统配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(PointsSystemConfigViewModel)).Schema<PointsSystemConfigViewModel>())
                            .Response(200, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAvailableVariables")
                            .Tag("PointsSystem")
                            .Summary("获取可用变量")
                            .Description("Get the availableVariables")
                            .Response(200, r => r.Description("Succeeded or failed"))));

            Describe["RetrievePlayerPoints"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrievePlayerPoints")
                            .Tag("PointsSystem")
                            .Summary("获取玩家积分信息")
                            .Parameter(new Parameter() { Name = "steamId", In = ParameterIn.Query, Required = true })
                            .Description("If the parameter steamId is null then return a failed result, otherwise returns player's points information")
                            .Response(r => r.Schema<T_Points>().Description("The player's points information"))));

            Describe["UpdatePlayerPoints"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdatePlayerPoints")
                            .Tag("PointsSystem")
                            .Summary("更新玩家积分信息")
                            .Description("")
                            .BodyParameter(p => p.Description("A points information object").Name(nameof(PointsInfoViewModel)).Schema<PointsInfoViewModel>())
                            .Response(200, r => r.Description("Succeeded or failed"))));

        }
    }
}