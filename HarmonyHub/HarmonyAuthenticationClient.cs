using agsXMPP;
using agsXMPP.protocol.client;
using System.Text.RegularExpressions;

namespace HarmonyHub
{
    public class HarmonyAuthenticationClient : HarmonyClient
    {
        private string _sessionToken;

        public HarmonyAuthenticationClient(string ipAddress, int port)
            : base(ipAddress, port, "guest")
        {
            Xmpp.OnIq += OnIq;
        }

        public string SwapAuthToken(string token)
        {
            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(HarmonyDocuments.LogitechPairDocument(token));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);

            return _sessionToken;
        }

        void OnIq(object sender, IQ iq)
        {
            if (iq.HasTag("oa"))
            {
                if (iq.InnerXml.Contains("errorcode=\"200\""))
                {
                    const string identityRegEx = "identity=([A-Z0-9]{8}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{12}):status";
                    var regex = new Regex(identityRegEx, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    var match = regex.Match(iq.InnerXml);
                    if (match.Success)
                    {
                        _sessionToken = match.Groups[1].ToString();
                    }

                    Wait = false;
                }
            }
        }
    }
}
