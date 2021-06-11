using IceCoffee.Common.Extensions;
using LSTY.Sdtd.PatronsMod.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class LocalizationModule : ApiModuleBase
    {
        public LocalizationModule()
        {
            HttpGet("/RetrieveLocalization", "RetrieveLocalization", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult(message: "Game is starting");
                }

                string itemName = Request.Query["itemName"];
                var dict = Localization.Dictionary;

                if (string.IsNullOrEmpty(itemName) || dict.ContainsKey(itemName) == false)
                {
                    return FailedResult(message: "The specified itemName does not exist");
                }

                string language = Request.Query["language"];

                if (string.IsNullOrEmpty(language))
                {
                    language = "schinese";
                }

                int languageIndex = Array.LastIndexOf(dict["KEY"], language);

                return SucceededResult(dict[itemName][languageIndex]);
            });

            HttpGet("/RetrieveKnownLanguages", "RetrieveKnownLanguages", _ =>
            {
                if (ModHelper.GameStartDone == false)
                {
                    return FailedResult(message: "Game is starting");
                }

                return SucceededResult(Localization.Dictionary["KEY"]);
            });
        }
    }
}
