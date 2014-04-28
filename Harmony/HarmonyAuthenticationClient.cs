using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Text.RegularExpressions;
using System.Threading;

namespace Harmony
{
    public class HarmonyAuthenticationClient : HarmonyClient
    {
        private const string HubUsername = "guest@x.com";
        private const string HubPassword = "guest";

        private string _sessionToken;

        private Document GetAuthMessage(string token)
        {
            var document = new Document { Namespace = "connect.logitech.com" };

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.connect/vnd.logitech.pair");
            element.Value = string.Format("token={0}:name={1}#{2}", token, "foo", "iOS6.0.1#iPhone");

            document.AddChild(element);
            return document;
        }

        public HarmonyAuthenticationClient(string ipAddress, int port, string token)
            : base(ipAddress, port)
        {
            Xmpp.OnIq += OnIq;
            Xmpp.Open(HubUsername, HubPassword);

            WaitForData(5);
        }

        public string SwapAuthToken(string token)
        {
            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(GetAuthMessage(token));
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
