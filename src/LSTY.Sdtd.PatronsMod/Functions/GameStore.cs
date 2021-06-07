using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.WebApi;
using System.Xml;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using IceCoffee.Common.Extensions;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    class GameStore : FunctionBase
    {
        private static readonly IPointsRepository _pointsRepository;
        private static readonly IGoodsRepository _goodsRepository;

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListCmd { get; set; } = "shop";

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListPreTips { get; set; } = "[00FF00]Goods list:";

        [ConfigNode(XmlNodeType.Attribute)]
        public string QueryListTips { get; set; } = "[00FF00]{goodsName} {buyCmd} {count} {quality} {price} {type}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string BuySuccessfullyTips { get; set; } = "[00FF00]Buy successfully";

        [ConfigNode(XmlNodeType.Attribute)]
        public string PointsNotEnoughTips { get; set; } = "[00FF00]Not enough points! Points required: {price}";

        [ConfigNode(XmlNodeType.Attribute)]
        public string GoodsNoFoundTips { get; set; } = "[00FF00]Goods no found";

        static GameStore()
        {
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();
            _goodsRepository = IocContainer.Resolve<IGoodsRepository>();
        }

        public GameStore()
        {
            availableVariables.AddRange(new string[]
            {
                "{goodsName}",
                "{buyCmd}",
                "{count}",
                "{quality}",
                "{price}",
                "{contentType}"
            });
        }

        private string FormatCmd(OnlinePlayer player, string message, T_Goods goods)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(player, message));

            return builder
                .Replace("{goodsName}", goods.Name)
                .Replace("{buyCmd}", goods.BuyCmd)
                .Replace("{count}", goods.Count.ToString())
                .Replace("{quality}", goods.Quality.ToString())
                .Replace("{price}", goods.Price.ToString())
                .Replace("{contentType}", goods.ContentType)
                .ToString();
        }

        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            string steamId = player.SteamId;
            if (string.Equals(message, QueryListCmd, StringComparison.OrdinalIgnoreCase))
            {
                var goodsList = _goodsRepository.Query(whereBy: "Price DASC");

                if (goodsList.Any() == false)
                {
                    ModHelper.SendMessageToPlayer(steamId, GoodsNoFoundTips);
                }
                else
                {
                    ModHelper.SendMessageToPlayer(steamId, QueryListPreTips);

                    int index = 1;
                    foreach (var item in goodsList)
                    {
                        ++index;
                        ModHelper.SendMessageToPlayer(steamId,
                            string.Format("[00FF00]{0}. {1}", index, FormatCmd(player, QueryListTips, item)));
                    }
                }

                return true;
            }
            else
            {
                var goods = _goodsRepository.QueryById("BuyCmd", message).FirstOrDefault();
                if (goods == null)
                {
                    return false;
                }
                else
                {
                    int playerPoints = _pointsRepository.QueryPointsCountBySteamId(steamId);
                    if (playerPoints < goods.Price)
                    {
                        ModHelper.SendMessageToPlayer(steamId, FormatCmd(player, PointsNotEnoughTips, goods));
                    }
                    else
                    {
                        _pointsRepository.DeductPlayerPoints(steamId, goods.Price);

                        switch (goods.Name)
                        {
                            case ContentTypes.Item:
                                ModHelper.GiveItem(player.EntityId, goods.Content, goods.Count, goods.Quality);
                                break;
                            case ContentTypes.Block:
                                ModHelper.GiveItem(player.EntityId, goods.Content, goods.Count);
                                break;
                            case ContentTypes.Entity:
                                for (int i = 0; i < goods.Count; ++i)
                                {
                                    ModHelper.SpawnEntity(player.EntityId, goods.Content);
                                }
                                break;
                            case ContentTypes.Command:
                                for (int i = 0; i < goods.Count; ++i)
                                {
                                    SdtdConsole.Instance.ExecuteSync(FormatCmd(player, goods.Content, goods), null);
                                }
                                break;
                            default:
                                throw new Exception("Invalid goods type");
                        }

                        ModHelper.SendMessageToPlayer(steamId, FormatCmd(player, BuySuccessfullyTips, goods));

                        CustomLogger.Info("Player: {0} steamId: {1} bought: {2}", player.Name, player.SteamId, goods.Name);
                    }

                    return true;
                }
            }
        }
    }
}
