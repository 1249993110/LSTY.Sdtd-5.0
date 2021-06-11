using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Internal;
using Nancy;
using System.IO;
using System.Reflection;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", _ =>
            {
                return Response.AsFile(ModHelper.ModPath + "/wwwroot/index.html", "text/html; charset=utf-8");
            });
        }
    }
}
