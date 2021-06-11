using LSTY.Sdtd.PatronsMod.MapRendering;
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
            return new[] { "ty-RenderMap" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            MapRender.Instance.RenderFullMap();

            Log("Render map done");
        }
    }
}