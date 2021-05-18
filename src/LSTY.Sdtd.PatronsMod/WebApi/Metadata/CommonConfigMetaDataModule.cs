using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using LSTY.Sdtd.PatronsMod.WebApi.ViewModels;
using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class CommonConfigMetaDataModule : MetadataModule<PathItem>
    {
        public CommonConfigMetaDataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<WebConfig>();
            modelCatalog.AddModel<CommonConfigViewModel>();

            Describe["RetrieveCommonConfig"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveCommonConfig")
                           .Tag("CommonConfig")
                           .Summary("获取公共配置")
                           .Description("")
                           .Response(r => r.Schema<CommonConfigViewModel>().Description("Common config"))));

            Describe["UpdateCommonConfig"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("UpdateCommonConfig")
                            .Tag("CommonConfig")
                            .Summary("更新公共配置")
                            .Description("")
                            .BodyParameter(p => p.Description("A config object").Name(nameof(CommonConfigViewModel)).Schema<CommonConfigViewModel>())
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
        }
    }
}
