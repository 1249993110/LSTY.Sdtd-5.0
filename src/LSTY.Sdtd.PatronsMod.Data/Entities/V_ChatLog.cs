using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class V_ChatLog : EntityBase
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public string SteamId { get; set; }

        public int EntityId { get; set; }

        public int ChatType { get; set; }

        public string Message { get; set; }

        public string PlayerName { get; set; }
    }
}