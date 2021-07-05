using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class T_KillReward : EntityBase
    {
        [PrimaryKey]
        public string Id { get; set; }

        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        public string SteamIdOrEntityName { get; set; }

        public string FriendlyName { get; set; }

        public string RewardContent { get; set; }

        public int RewardCount { get; set; }

        public int RewardQuality { get; set; }

        public string RewardContentType { get; set; }

        public string SpawnedTips { get; set; }

        public string KilledTips { get; set; }
    }
}
