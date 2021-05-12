using LSTY.Sdtd.PatronsMod.Primitives;
using LSTY.Sdtd.PatronsMod.WebApi.ViewModels;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Modules
{
    public class FunctionManageModule : ApiModuleBase
    {
        public FunctionManageModule()
        {
            HttpGet("/RetrieveFunctionState", "RetrieveFunctionState", _ =>
            {
                List<FunctionManageViewModel> data = new List<FunctionManageViewModel>();

                foreach (var function in FunctionManager.Functions)
                {
                    data.Add(new FunctionManageViewModel()
                    {
                        FunctionName = function.FunctionName,
                        IsEnabled = function.IsEnabled
                    });
                }

                return SucceededResult(data);
            });


            HttpPost("/UpdateFunctionState", "UpdateFunctionState", _ =>
            {
                string query = Request.Query["isBatch"];
                bool isBatch = string.IsNullOrEmpty(query) ? false : Convert.ToBoolean(query);
                if (isBatch)
                {
                    var data = this.Bind<IEnumerable<FunctionManageViewModel>>();
                    IFunction function;
                    foreach (var item in data)
                    {
                        function = FunctionManager.Functions.FirstOrDefault(p => p.FunctionName == item.FunctionName);
                        if (function == null)
                        {
                            return FailedResult(message: $"function: {function.FunctionName} not exist");
                        }
                        else
                        {
                            function.IsEnabled = item.IsEnabled;
                        }
                    }

                    return SucceededResult();
                }
                else
                {
                    var data = this.Bind<FunctionManageViewModel>();
                    var function = FunctionManager.Functions.FirstOrDefault(p => p.FunctionName == data.FunctionName);
                    if (function == null)
                    {
                        return FailedResult(message: "function not exist");
                    }

                    function.IsEnabled = data.IsEnabled;

                    return SucceededResult();
                }
            });
        }
    }
}
