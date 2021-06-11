using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.MapRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.Commands
{
	public class RestartServer : ConsoleCmdBase
	{
		public override string GetDescription()
		{
			return "restart server, use args";
		}

		public override string[] GetCommands()
		{
			return new[] { "ty-rs", "ty-RestartServer" };
		}

        public override string GetHelp()
        {
			return "Usage:\n" +
				"ty-rs: restart server by shutdown" +
				"ty-rs force: force restart server";
		}

        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
		{
			Log("Server is restarting");

			if (args.Count > 0)
            {
				if(args[0] == "force")
                {
					ModHelper.RestartServer(true);
				}
            }
            else
            {
				ModHelper.RestartServer();
			}
		}
	}
}
