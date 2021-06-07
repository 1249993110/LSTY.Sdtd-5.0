using System.Collections.Generic;

namespace LSTY.Sdtd.PatronsMod.Commands
{
    public class SayToPlayer : ConsoleCmdBase
    {
        public override string GetDescription()
        {
            return "Send a message to a single player";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "   ty-pm <steam id / entity id / player name> <message>\n" +
                   "Send a PM to the player given by the steam id or entity id or player name (as given by e.g. \"lpi\").";
        }

        public override string[] GetCommands()
        {
            return new[] { "ty-SayPlayer", "ty-pm" };
        }

        private void InternalExecute(ClientInfo sender, List<string> args)
        {
            if (args.Count < 2)
            {
                Log("Usage: sayplayer <steamId|entityId|playerName> <message>");
                return;
            }

            string message = args[1];

            ClientInfo receiver = ConsoleHelper.ParseParamIdOrName(args[0]);
            if (receiver == null)
            {
                Log("SteamId or entityId or playerName not found.");
            }
            else
            {
                ModHelper.SendMessage(receiver, sender, message);
            }
        }

        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            // From game client.
            if (senderInfo.RemoteClientInfo != null)
            {
                InternalExecute(senderInfo.RemoteClientInfo, args);
            }
            // From console.
            else
            {
                InternalExecute(null, args);
            }
        }
    }
}
