using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    public class LocalizationMetaDataModule : MetadataModule<PathItem>
    {
        public LocalizationMetaDataModule()
        {
            Describe["RetrieveLocalization"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveLocalization")
                           .Tag("Localization")
                           .Summary("获取本地化")
                           .Description("")
                           .Parameters(new Parameter[] 
                           { 
                               new Parameter() { Name = "itemName", In = ParameterIn.Query, Required = true }, 
                               new Parameter() { Name = "language", In = ParameterIn.Query, Default = "schinese" } 
                           })
                           .Response(r => r.Description("Localization string"))));

            Describe["RetrieveKnownLanguages"] = description => description.AsSwagger(
               with => with.Operation(
                   op => op.SecurityRequirement(SecuritySchemes.ApiKey)
                           .OperationId("RetrieveKnownLanguages")
                           .Tag("Localization")
                           .Summary("获取已知语言")
                           .Description("")
                           .Response(r => r.Description("Known languages"))));
        }
    }
}
