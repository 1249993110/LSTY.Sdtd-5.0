using LSTY.Sdtd.PatronsMod.LiveData;
using Nancy.Swagger;
using Nancy.Swagger.Services;

namespace LSTY.Sdtd.PatronsMod.WebApi.ModelDataProviders
{
    public class OnlinePlayerModelDataProvider : ISwaggerModelDataProvider
    {
        public SwaggerModelData GetModelData()
        {
            return SwaggerModelData.ForType<OnlinePlayer>(with =>
            {
                with.Description("Online player in game");

                with.Property(x => x.SteamId)
                    .Description("The player's steamId");

                with.Property(x => x.Name)
                   .Description("The player's name");

                with.Property(x => x.EntityId)
                   .Description("The player's entityId");
            });
        }
    }
}
