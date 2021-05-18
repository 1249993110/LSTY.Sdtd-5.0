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
    public class TeleportHomeMetadataModule : MetadataModule<PathItem>
    {
        public TeleportHomeMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<T_HomePosition>();
            modelCatalog.AddModel<TeleportHomeConfigViewModel>();
            modelCatalog.AddModel<HomePositionViewModelBase>();
            modelCatalog.AddModel<HomePositionViewModel>();

            Describe["RetrieveTeleportHomeConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveTeleportHomeConfig")
                            .Tag("TeleportHome")
                            .Summary("获取私人回家配置")
                            .Description("")
                            .Response(r => r.Schema<TeleportHomeConfigViewModel>().Description("The config of TeleportHome"))));

            Describe["UpdateTeleportHomeConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateTeleportHomeConfig")
                            .Tag("TeleportHome")
                            .Summary("更新私人回家配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(TeleportHomeConfigViewModel)).Schema<TeleportHomeConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_TeleportHome"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAvailableVariables_TeleportHome")
                            .Tag("TeleportHome")
                            .Summary("获取可用变量")
                            .Description("Get the availableVariables")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            
            Describe["CreateHomePosition"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("CreateHomePosition")
                            .Tag("TeleportHome")
                            .Summary("创建私人回家点")
                            .BodyParameter(p => p.Description("A object").Name(nameof(HomePositionViewModelBase)).Schema<HomePositionViewModelBase>())
                            .Description("")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveHomePosition"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveHomePosition")
                            .Tag("TeleportHome")
                            .Summary("获取私人回家点")
                            .Parameter(new Parameter() { Name = "homePositionId", In = ParameterIn.Query })
                            .Description("If the parameter home position id is null then return all home positions, otherwise returns the specified home position")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["UpdateHomePosition"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateHomePosition")
                            .Tag("TeleportHome")
                            .Summary("更新私人回家点")
                            .Description("")
                            .BodyParameter(p => p.Description("A object").Name(nameof(HomePositionViewModel)).Schema<HomePositionViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
        }
    }
}
