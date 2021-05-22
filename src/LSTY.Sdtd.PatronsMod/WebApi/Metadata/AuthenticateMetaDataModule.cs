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
    public class AuthenticateMetaDataModule : MetadataModule<PathItem>
    {
        public AuthenticateMetaDataModule()
        {
            Describe["Authenticate"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.OperationId("Authenticate")
                            .Tag("Authenticate")
                            .Summary("认证")
                            .Parameter(new Parameter() { Name = WebConfig.AuthKeyName, In = ParameterIn.Header })
                            .Description("Check " + WebConfig.AuthKeyName)
                            .Response((int)HttpStatusCode.OK, r => r.Description("Succeeded"))
                            .Response((int)HttpStatusCode.Unauthorized, r => r.Description("Failed"))));
        }
    }
}
