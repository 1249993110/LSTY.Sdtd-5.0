using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class T_CDKey : EntityBase
    {
        [PrimaryKey, IgnoreUpdate, IgnoreInsert]
        public int Id { get; set; }

        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        [Column("[Key]")]
        public string Key { get; set; }

        public int LimitUseOnceEachPlayer { get; set; }

        public int MaxExchangeCount { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string ItemName { get; set; }

        public string ItemContent { get; set; }

        public int ItemCount { get; set; }

        public int ItemQuality { get; set; }

        public string ContentType { get; set; }
    }
}
