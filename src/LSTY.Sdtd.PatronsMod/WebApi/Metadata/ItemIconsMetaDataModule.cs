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
    public class ItemIconsMetaDataModule : MetadataModule<PathItem>
    {
        public ItemIconsMetaDataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<GameNoticeConfigViewModel>();

            Describe["itemicons"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.OperationId("itemicons")
                            .Tag("itemicons")
                            .Summary("获取服务器的图标")
                            .Description("Get server' icons. You can get tinted icon by use like airConditioner__00FF00.png")
                            .Parameter(new Parameter() { Name = "iconName", In = ParameterIn.Path, Required = true })
                            .Response((int)HttpStatusCode.NotFound, r => r.Description("The icon not found"))
                            .Response((int)HttpStatusCode.OK, r => r.Description("The icon"))));
        }
    }
}