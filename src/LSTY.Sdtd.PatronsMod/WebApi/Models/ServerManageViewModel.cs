using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.Models
{
    public class ConsoleCommand
    {
        public string[] Command { get; set; }

        public string Description { get; set; }

        public string Help { get; set; }
    }

    public class GameStats
    {
        public Gametime Gametime { get; set; }

        public int Players { get; set; }

        public int Hostiles { get; set; }

        public int Animals { get; set; }
    }

    public class Gametime
    {
        public int Days { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }
    }

    public class EntityLocation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Position Position { get; set; }
    }

    public class PlayersLocation
    {
        public string SteamId { get; set; }

        public int EntityId { get; set; }

        public string Name { get; set; }

        public bool Online { get; set; }

        public Position Position { get; set; }
    }

    public class ClaimOwner
    {
        public bool Claimactive { get; set; }

        public string SteamId { get; set; }

        public int EntityId { get; set; }

        public string PlayerName { get; set; }

        public List<Position> Claims { get; set; }
    }

    public class LandClaims
    {
        public int Claimsize { get; set; }

        public List<ClaimOwner> ClaimOwners { get; set; }
    }
}
