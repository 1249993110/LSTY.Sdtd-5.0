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

                string language = Request.Query["language"];

                if (string.IsNullOrEmpty(language))
                {
                    language = "schinese";
                }

                int languageIndex = Array.LastIndexOf(dict["KEY"], language);

                if(languageIndex < 0)
                {
                    return FailedResult(message: "The specified language does not exist");
                }

                if (string.IsNullOrEmpty(itemName))
                {
                    return SucceededResult(dict.ToDictionary(p => p.Key, p => p.Value[languageIndex]));
                }

                if (dict.ContainsKey(itemName) == false)
                {
                    return FailedResult(message: "The specified itemName does not exist");
                }

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
