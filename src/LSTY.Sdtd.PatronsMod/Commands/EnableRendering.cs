using LSTY.Sdtd.PatronsMod.WebApi.MapRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.Commands
{
	public class EnableRendering : ConsoleCmdBase
	{
		public override string GetDescription()
		{
			return "enable/disable live map rendering";
		}

		public override string[] GetCommands()
		{
			return new[] { "ty-enablerendering" };
		}

		public override void Execute(List<string> args, CommandSenderInfo senderInfo)
		{
			if (args.Count != 1)
			{
				Log("Current state: " + MapRendering.RenderingEnabled);
				return;
			}

			MapRendering.RenderingEnabled = args[0].Equals("1");
			Log("Set live map rendering to " + args[0].Equals("1"));
		}
	}
}
