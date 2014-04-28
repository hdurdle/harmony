using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Text.RegularExpressions;
using System.Threading;

namespace Harmony
{
    class ConfigClient
    {
        private readonly XmppClientConnection _xmpp;
        private bool _wait;

        public string Config { get; set; }
        public string SessionToken { get; set; }

        public ConfigClient(string ipAddress, int port, string token)
        {
            SessionToken = token;
            string username = string.Format("{0}@x.com", token);

            _xmpp = new HarmonyClientConnection(ipAddress, port);
            _xmpp.OnLogin += delegate { _wait = false; };
            _xmpp.OnIq += OnIq;
            _xmpp.Open(username, token);

            _wait = true;
            int i = 0;
            do
            {
                i++;
                if (i == 10)
                    _wait = false;
                Thread.Sleep(500);
            } while (_wait);
        }

        private Document GetConfigMessage()
        {
            var document = new Document { Namespace = "connect.logitech.com" };

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?config");

            // element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?getCurrentActivity");
            // element.Value = string.Format("token={0}:name={1}#{2}", token, "1vm7ATw/tN6HXGpQcCs/A5MkuvI", "iOS6.0.1#iPhone");

            document.AddChild(element);
            return document;
        }

        public void GetConfig()
        {
            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(GetConfigMessage());
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(_xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            _wait = true;
            int i = 0;
            do
            {
                i++;
                if (i == 10)
                    _wait = false;
                Thread.Sleep(500);
            } while (_wait);
        }

        void OnIq(object sender, IQ iq)
        {
            if (iq.HasTag("oa"))
            {
                if (iq.InnerXml.Contains("errorcode=\"200\""))
                {
                    const string identityRegEx = "errorstring=\"OK\">(.*)</oa>";
                    var regex = new Regex(identityRegEx, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    var match = regex.Match(iq.InnerXml);
                    if (match.Success)
                    {
                        Config = match.Groups[1].ToString();
                    }
                }
            }
            _wait = false;
        }
    }
}
