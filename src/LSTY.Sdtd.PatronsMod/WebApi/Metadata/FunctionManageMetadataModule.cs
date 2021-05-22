using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    /// <summary>
    /// The class name must end with MetadataModule
    /// </summary>
    public class FunctionManageMetadataModule : MetadataModule<PathItem>
    {
        public FunctionManageMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModels(typeof(FunctionManageViewModel), typeof(IEnumerable<FunctionManageViewModel>));
      
            Describe["RetrieveFunctionState"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                            .OperationId("RetrieveFunctionState")
                            .Tag("FunctionManage")
                            .Summary("获取功能状态")
                            .Description("")
                            .Response(r => r.Schema<IEnumerable<FunctionManageViewModel>>(modelCatalog).Description("Function state"))));

            Describe["UpdateFunctionState"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("UpdateFunctionState")
                           .Tag("FunctionManage")
                           .Summary("更新功能状态")
                           .Description("")
                           .Parameter(new Parameter() { Name = "isBatch", In = ParameterIn.Query, Description = "Whether batch update, the default value is false", Required = false })
                           .BodyParameter(p => p.Description("A config object").Name(nameof(FunctionManageViewModel)).Schema<FunctionManageViewModel>())
                           .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded or failed"))));
        }
    }
}
