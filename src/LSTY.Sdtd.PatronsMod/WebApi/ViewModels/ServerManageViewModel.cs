using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.ViewModels
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
}
