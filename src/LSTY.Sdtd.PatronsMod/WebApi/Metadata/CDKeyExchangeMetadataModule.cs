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
    public class CDKeyExchangeMetadataModule : MetadataModule<PathItem>
    {
        public CDKeyExchangeMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<CDKeyExchangeConfigViewModel>();
            modelCatalog.AddModel<CDKeyExchangeViewModelBase>();
            modelCatalog.AddModel<IEnumerable<CDKeyExchangeViewModelBase>>();
            modelCatalog.AddModel<CDKeyExchangeViewModel>();
            modelCatalog.AddModel<DeleteQueryParamOfString>();
            modelCatalog.AddModel<T_CDKeyViewModel>();
            modelCatalog.AddModel<IEnumerable<T_CDKeyViewModel>>();
            modelCatalog.AddModel<T_CDKeyExchangeLog>();
            modelCatalog.AddModel<IEnumerable<T_CDKeyExchangeLog>>();

            Describe["RetrieveCDKeyExchangeConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveCDKeyExchangeConfig")
                            .Tag("CDKeyExchange")
                            .Summary("获取CDKey兑换配置")
                            .Description("")
                            .Response(r => r.Schema<CDKeyExchangeConfigViewModel>().Description("The config of CDKeyExchange"))));

            Describe["UpdateCDKeyExchangeConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateCDKeyExchangeConfig")
                            .Tag("CDKeyExchange")
                            .Summary("更新CDKey兑换配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(CDKeyExchangeConfigViewModel)).Schema<CDKeyExchangeConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveAvailableVariables_CDKeyExchange"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveAvailableVariables_CDKeyExchange")
                            .Tag("CDKeyExchange")
                            .Summary("获取可用变量")
                            .Description("Get the availableVariables")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
            
            Describe["CreateCDKey"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("CreateCDKey")
                            .Tag("CDKeyExchange")
                            .Summary("创建CDKey兑换项")
                            .Parameter(new Parameter() { Name = "isBatch", In = ParameterIn.Query, Description = "Whether batch create, the default value is false", Required = false })
                            .BodyParameter(p => p.Description("A array").Name(nameof(CDKeyExchangeViewModelBase)).Schema<CDKeyExchangeViewModelBase>())
                            .Description("")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveCDKeyPaged"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveCDKeyPaged")
                            .Tag("CDKeyExchange")
                            .Summary("获取CDKey兑换项")
                            .BodyParameter(p => p.Description("Query params").Name(nameof(PaginationQueryParams)).Schema<PaginationQueryParams>())
                            .Description("")
                            .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<T_CDKeyViewModel>>(modelCatalog).Description("Succeeded or failed"))));

            Describe["UpdateCDKey"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateCDKey")
                            .Tag("CDKeyExchange")
                            .Summary("更新CDKey兑换项")
                            .Description("")
                            .BodyParameter(p => p.Description("A object").Name(nameof(CDKeyExchangeViewModel)).Schema<CDKeyExchangeViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["DeleteCDKey"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("DeleteCDKey")
                            .Tag("CDKeyExchange")
                            .Summary("删除CDKey兑换项")
                            .BodyParameter(p => p.Description("A array").Name(nameof(DeleteQueryParamOfString)).Schema<DeleteQueryParamOfString>())
                            .Description("Delete item by ids")
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));

            Describe["RetrieveCDKeyExchangeLogPaged"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveCDKeyExchangeLogPaged")
                            .Tag("CDKeyExchange")
                            .Summary("获取CDKey兑换记录")
                            .BodyParameter(p => p.Description("Query params").Name(nameof(PaginationQueryParams)).Schema<PaginationQueryParams>())
                            .Description("")
                            .Response((int)HttpStatusCode.OK, r => r.Schema<IEnumerable<T_CDKeyExchangeLog>>(modelCatalog).Description("Succeeded or failed"))));

        }
    }
}
