using Nancy.Swagger.Services;
using Swagger.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.TagProviders
{
    public class KillRewardTagProvider : ISwaggerTagProvider
    {
        public Tag GetTag()
        {
            return new Tag()
            {
                Description = "击杀悬赏",
                Name = "KillReward"
            };
        }
    }
}
