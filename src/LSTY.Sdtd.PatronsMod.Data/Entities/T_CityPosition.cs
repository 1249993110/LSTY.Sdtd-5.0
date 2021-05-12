using IceCoffee.DbCore.OptionalAttributes;
using IceCoffee.DbCore.Primitives.Entity;
using System;

namespace LSTY.Sdtd.PatronsMod.Data.Entities
{
    public class T_CityPosition : EntityBase
    {
        [PrimaryKey]
        public string Id { get; set; }

        [IgnoreUpdate, IgnoreInsert]
        public DateTime CreatedDate { get; set; }

        public string CityName { get; set; }

        public string Command { get; set; }

        public int PointsRequired { get; set; }

        public string Position { get; set; }
    }
}