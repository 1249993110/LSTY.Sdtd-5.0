using LSTY.Sdtd.PatronsMod.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace LSTY.Sdtd.PatronsMod.WebSocket
{
    public class WebSocketSession : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                List<string> executeResult = SdtdConsole.Instance.ExecuteSync(e.Data, new ClientInfo() { playerId = "LSTY.WebSocket" });
                Send(string.Join(Environment.NewLine, executeResult));
            }
            else
            {
                Send("Invalid data");
                CloseAsync();
            }
        }

        protected override void OnOpen()
        {
            string accessToken = Context.QueryString.Get(WebConfig.AuthKeyName);
            if (accessToken == null || accessToken != FunctionManager.CommonConfig.WebConfig.AccessToken)
            {
                Send("Invalid access-token");
                CloseAsync();
            }
        }
    }
}
