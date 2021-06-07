using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class T_Lottery : EntityBase
    {
        [PrimaryKey]
        public string Id { get; set; }

        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public int Count { get; set; }

        public int Quality { get; set; }

        public string ContentType { get; set; }
    }
}