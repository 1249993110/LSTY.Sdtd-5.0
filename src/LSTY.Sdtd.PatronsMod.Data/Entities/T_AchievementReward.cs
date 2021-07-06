using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class T_AchievementReward : EntityBase
    {
        [PrimaryKey, IgnoreUpdate, IgnoreInsert]
        public int Id { get; set; }

        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string TriggerVariable { get; set; }

        public int TriggerRequiredCount { get; set; }

        public string RewardContent { get; set; }

        public int RewardCount { get; set; }

        public int RewardQuality { get; set; }

        public string ContentType { get; set; }
    }
}
