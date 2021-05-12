using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.WebApi.Metadata
{
    /// <summary>
    /// The class name must end with MetadataModule
    /// </summary>
    public class HomeMetadataModule : MetadataModule<PathItem>
    {
        public HomeMetadataModule()
        {
        }
    }
}
