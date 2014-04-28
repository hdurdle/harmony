using System;
using System.Diagnostics;
using agsXMPP;

namespace Harmony
{
    /// <summary>
    /// Extended XmppClientConnection with default settings compatible with Harmony Hub.
    /// </summary>
    class HarmonyClientConnection : XmppClientConnection
    {
        public HarmonyClientConnection(string ipAddress, int port)
            : base(ipAddress, port)
        {
            UseStartTLS = false;
            UseSSL = false;
            UseCompression = false;

            AutoResolveConnectServer = false;
            AutoAgents = false;
            AutoPresence = true;
            AutoRoster = true;

            OnSaslStart += HarmonyClientConnection_OnSaslStart;
            OnSocketError += HarmonyClientConnection_OnSocketError;
        }

        void HarmonyClientConnection_OnSocketError(object sender, System.Exception ex)
        {
            throw ex;
        }

        static void HarmonyClientConnection_OnSaslStart(object sender, agsXMPP.sasl.SaslEventArgs args)
        {
            args.Auto = false;
            args.Mechanism = "PLAIN";
        }
    }
}
