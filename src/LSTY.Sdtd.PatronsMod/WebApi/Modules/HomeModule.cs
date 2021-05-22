using IceCoffee.Common.Extensions;
using Nancy;
using System.IO;
using System.Reflection;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class HomeModule : NancyModule
    {
        private static readonly string _indexPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\wwwroot\\index.html";

        public HomeModule()
        {
            Get("/", _ =>
            {
                return Response.AsFile(_indexPath, "text/html; charset=utf-8");
            });
        }
    }
}
