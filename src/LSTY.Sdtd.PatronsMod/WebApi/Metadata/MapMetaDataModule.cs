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
    public class MapMetaDataModule : MetadataModule<PathItem>
    {
        public MapMetaDataModule()
        {
            Describe["map"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.OperationId("map")
                            .Tag("Map")
                            .Summary("切片地图")
                            .Description("Get tile map")
                            .Parameters(new Parameter[]
                            {
                                new Parameter() { Name = "z", In = ParameterIn.Path, Required = true, Description="zoom" },
                                new Parameter() { Name = "x", In = ParameterIn.Path, Required = true },
                                new Parameter() { Name = "y", In = ParameterIn.Path, Required = true }
                            })
                            .Response((int)HttpStatusCode.OK, r => r.Description("The tile image"))));
        }
    }
}