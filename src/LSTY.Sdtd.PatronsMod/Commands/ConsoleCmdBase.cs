using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.Commands
{
    public abstract class ConsoleCmdBase : ConsoleCmdAbstract
    {
		protected static ClientInfo ParseParamId(string id)
		{
			var clients = ConnectionManager.Instance.Clients;
            if (id.Length == 17 && ulong.TryParse(id, out ulong _))
            {
                ClientInfo clientInfo = clients.ForPlayerId(id);
                if (clientInfo != null)
                {
                    return clientInfo;
                }
            }

            if (int.TryParse(id, out int entityId))
            {
                ClientInfo clientInfo2 = clients.ForEntityId(entityId);
                if (clientInfo2 != null)
                {
                    return clientInfo2;
                }
            }

            return null;
		}

		protected virtual void Log(string line)
        {
            SdtdConsole.Instance.Output(CustomLogger.Prefix + line);
        }

        protected virtual void Log(string line, params object[] args)
        {
            SdtdConsole.Instance.Output(CustomLogger.Prefix + string.Format(line, args));
        }
    }
}
