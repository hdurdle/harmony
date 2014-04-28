using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using agsXMPP;

namespace Harmony
{
    public class HarmonyClient
    {
        protected bool Wait;
        protected XmppClientConnection Xmpp;

        protected HarmonyClient(string ipAddress, int port)
        {
            Xmpp = new HarmonyClientConnection(ipAddress, port);
            Xmpp.OnLogin += delegate { Wait = false; };

        }

        protected void WaitForData(int timeoutSeconds)
        {
            Wait = true;
            int i = 0;
            do
            {
                i++;
                if (i == (timeoutSeconds * 2))
                    Wait = false;
                Thread.Sleep(500);
            } while (Wait);

        }
    }
}
