using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class T_TeleRecord : EntityBase
    {
        [PrimaryKey, IgnoreUpdate, IgnoreInsert]
        public int Id { get; set; }

        public string SteamId { get; set; }

        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        public string TargetType { get; set; }

        public string DestinationName { get; set; }

        public string Position { get; set; }
    }
}