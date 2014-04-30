using agsXMPP;

namespace HarmonyHub
{
    /// <summary>
    /// An XmppClientConnection for connecting to the Logitech Harmony Hub.
    /// </summary>
    public class HarmonyClientConnection : XmppClientConnection
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
