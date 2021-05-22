using LSTY.Sdtd.PatronsMod.WebApi.Modules;
using LSTY.Sdtd.PatronsMod.WebApi.Models;
using Nancy.Swagger;
using Nancy.Swagger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.ModelDataProviders
{
    public class PointsSystemModelDataProvider : ISwaggerModelDataProvider
    {
        public SwaggerModelData GetModelData()
        {
            return SwaggerModelData.ForType<PointsSystemConfigViewModel>(with =>
            {
                with.Description("Points system config");

                with.Property(x => x.IsEnabled)
                    .Description("Function is or no enabled");

                with.Property(x => x.FunctionName)
                    .Description("The name of function");

                with.Property(x => x.SignCmd)
                    .Description("Sign command");

                with.Property(x => x.RewardCount)
                    .Description("Reward points");

                with.Property(x => x.SignInterval)
                    .Description("Sign interval");

                with.Property(x => x.InitialCount)
                    .Description("Initial points count");

                with.Property(x => x.QueryPointsCmd)
                    .Description("Query points command");

                with.Property(x => x.QueryPointsTips)
                    .Description("Query points tips");
                with.Property(x => x.SignFailTips)
                   .Description("Sign fail tips");

                with.Property(x => x.SignSucceedTips)
                    .Description("Sign succeed tips");

                with.Property(x => x.NeverSignInTips)
                   .Description("Never signed in tips");

            });
        }
    }
}
