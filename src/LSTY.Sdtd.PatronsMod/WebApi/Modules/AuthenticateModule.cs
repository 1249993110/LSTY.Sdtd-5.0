using Nancy;
using System.IO;
using System.Reflection;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class AuthenticateModule : ApiModuleBase
    {
        public AuthenticateModule()
        {
            Get("/Authenticate", _ =>
            {
                return SucceededResult();

            }, null, "Authenticate");
        }
    }
}
