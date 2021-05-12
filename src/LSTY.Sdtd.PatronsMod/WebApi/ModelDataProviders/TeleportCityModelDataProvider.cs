using LSTY.Sdtd.PatronsMod.WebApi.Modules;
using LSTY.Sdtd.PatronsMod.WebApi.ViewModels;
using Nancy.Swagger;
using Nancy.Swagger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.ModelDataProviders
{
    public class TeleportCityModelDataProvider : ISwaggerModelDataProvider
    {
        public SwaggerModelData GetModelData()
        {
            return SwaggerModelData.ForType<TeleportCityConfigViewModel>(with =>
            {
                with.Description("Points system config");

                with.Property(x => x.IsEnabled)
                    .Description("Function is or no enabled");

                with.Property(x => x.FunctionName)
                    .Description("The name of function");
            });
        }
    }
}
