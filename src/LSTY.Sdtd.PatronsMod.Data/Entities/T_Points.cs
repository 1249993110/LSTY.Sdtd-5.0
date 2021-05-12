using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class T_Points : EntityBase
    {
        [PrimaryKey]
        public string SteamId { get; set; }

        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        public int Count { get; set; }

        public int LastSignDay { get; set; }
    }
}