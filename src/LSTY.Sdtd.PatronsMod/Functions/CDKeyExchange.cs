using IceCoffee.Common.Timers;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.LiveData;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Linq;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.Data.Entities;
using System.Text;
using LSTY.Sdtd.PatronsMod.Data.IRepositories;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using IceCoffee.Common.Extensions;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class CDKeyExchange : FunctionBase
    {
        [ConfigNode(XmlNodeType.Attribute)]
        public string InvalidKeyTips { get; set; } = "[00FF00]Invalid key, key were expired or exceeded usage limit";

        [ConfigNode(XmlNodeType.Attribute)]
        public string ExchangeSuccessfullyTips { get; set; } = "[00FF00]You exchanged {itemName}";

        private readonly ICDKeyRepository _cdKeyRepository;
        private readonly ICDKeyExchangeLogRepository _cdKeyExchangeLogRepository;
        private readonly IPointsRepository _pointsRepository;
        public CDKeyExchange()
        {
            _cdKeyRepository = IocContainer.Resolve<ICDKeyRepository>();
            _cdKeyExchangeLogRepository = IocContainer.Resolve<ICDKeyExchangeLogRepository>();
            _pointsRepository = IocContainer.Resolve<IPointsRepository>();

            availableVariables.AddRange(new string[]
            {
                "{itemName}",
                "{itemCount}",
                "{itemQuality}",
                "{contentType}"
            });
        }

        private string FormatCmd(string message, OnlinePlayer player, T_CDKey item = null)
        {
            StringBuilder builder = new StringBuilder(base.FormatCmd(message, player));

            if (item != null)
            {
                builder.Replace("{itemName}", item.ItemName)
                .Replace("{itemCount}", item.ItemCount.ToString())
                .Replace("{itemQuality}", item.ItemQuality.ToString())
                .Replace("{contentType}", item.ContentType);
            }

            return builder.ToString();
        }

        [CatchException("Error in CDKeyExchange.OnPlayerChatHooked")]
        protected override bool OnPlayerChatHooked(OnlinePlayer player, string message)
        {
            var cdKey = _cdKeyRepository.QueryById("[Key]", message).FirstOrDefault();
            if (cdKey == null)
            {
                return false;
            }

            string steamId = player.SteamId;

            long count = _cdKeyExchangeLogRepository.QueryRecordCount("CDKey=@CDKey", new { CDKey = message });

            if (count >= cdKey.MaxExchangeCount)
            {
                ModHelper.SendMessageToPlayer(steamId, InvalidKeyTips);
                return true;
            }

            if (cdKey.LimitUseOnceEachPlayer == 1)
            {
                count = _cdKeyExchangeLogRepository.QueryRecordCount("CDKey=@CDKey AND SteamId=@SteamId",
                    new { CDKey = message, SteamId = steamId });
                if (count > 0)
                {
                    ModHelper.SendMessageToPlayer(steamId, InvalidKeyTips);
                    return true;
                }
            }

            switch (cdKey.ContentType)
            {
                case ContentTypes.Item:
                    ModHelper.GiveItem(player.EntityId, cdKey.ItemContent, cdKey.ItemCount, cdKey.ItemQuality);
                    break;
                case ContentTypes.Block:
                    ModHelper.GiveItem(player.EntityId, cdKey.ItemContent, cdKey.ItemCount);
                    break;
                case ContentTypes.Entity:
                    for (int i = 0; i < cdKey.ItemCount; ++i)
                    {
                        ModHelper.SpawnEntity(player.EntityId, cdKey.ItemContent);
                    }
                    break;
                case ContentTypes.Command:
                    for (int i = 0; i < cdKey.ItemCount; ++i)
                    {
                        foreach (string item in cdKey.ItemContent.Split(';'))
                        {
                            SdtdConsole.Instance.ExecuteSync(FormatCmd(item, player, cdKey), null);
                        }
                    }
                    break;
                case ContentTypes.Points:
                    _pointsRepository.IncreasePlayerPoints(steamId, cdKey.ItemCount);
                    break;
                default:
                    throw new Exception("Invalid content type");
            }

            ModHelper.SendMessageToPlayer(steamId, FormatCmd(ExchangeSuccessfullyTips, player, cdKey));

            _cdKeyExchangeLogRepository.Insert(new T_CDKeyExchangeLog()
            {
                CDKey = cdKey.Key,
                SteamId = steamId
            });

            CustomLogger.Info("Player: {0} steamId: {1} used cdkey {2} and get {3} by exchange",
                player.Name, steamId, cdKey.Key, cdKey.ItemName);
            return true;
        }
    }
}
