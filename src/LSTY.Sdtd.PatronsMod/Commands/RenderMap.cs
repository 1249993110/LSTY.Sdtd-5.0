using LSTY.Sdtd.PatronsMod.WebApi.MapRendering;
using System;
using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Commands
{
    public class RenderMap : ConsoleCmdBase
    {
        public override string GetDescription()
        {
            return "render the current map to a file";
        }

        public override string[] GetCommands()
        {
            return new[] { "ty-rendermap" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            MapRendering.Instance.RenderFullMap();

            Log("Render map done");
        }
    }
}