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
    public class TeleportCityMetadataModule : MetadataModule<PathItem>
    {
        public TeleportCityMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<T_CityPosition>();
            modelCatalog.AddModel<TeleportCityConfigViewModel>();
            modelCatalog.AddModel<CityPositionViewModelBase>();
            modelCatalog.AddModel<CityPositionViewModel>();

            Describe["RetrieveTeleportCityConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveTeleportCityConfig")
                            .Tag("TeleportCity")
                            .Summary("获取公共回城配置")
                            .Description("")
                            .Response(r => r.Schema<TeleportCityConfigViewModel>().Description("The config of TeleportCity"))));

            Describe["UpdateTeleportCityConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateTeleportCityConfig")
                            .Tag("TeleportCity")
                            .Summary("更新公共回城配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(TeleportCityConfigViewModel)).Schema<TeleportCityConfigViewModel>())
                            .Response(200, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAvailableVariables")
                            .Tag("TeleportCity")
                            .Summary("获取可用变量")
                            .Description("Get the availableVariables")
                            .Response(200, r => r.Description("Succeeded or failed"))));
            
            Describe["CreateCityPosition"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("CreateCityPosition")
                            .Tag("TeleportCity")
                            .Summary("创建公共回城点")
                            .BodyParameter(p => p.Description("A object").Name(nameof(CityPositionViewModelBase)).Schema<CityPositionViewModelBase>())
                            .Description("")
                            .Response(200, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveCityPosition"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveCityPosition")
                            .Tag("TeleportCity")
                            .Summary("获取公共回城点")
                            .Parameter(new Parameter() { Name = "cityPositionId", In = ParameterIn.Query })
                            .Description("If the parameter city position id is null then return all city positions, otherwise returns the specified city position")
                            .Response(200, r => r.Description("Succeeded or failed"))));

            Describe["UpdateCityPosition"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateCityPosition")
                            .Tag("TeleportCity")
                            .Summary("更新公共回城点")
                            .Description("")
                            .BodyParameter(p => p.Description("A object").Name(nameof(CityPositionViewModel)).Schema<CityPositionViewModel>())
                            .Response(200, r => r.Description("Succeeded or failed"))));
        }
    }
}
