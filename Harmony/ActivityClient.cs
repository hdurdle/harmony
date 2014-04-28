using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Text.RegularExpressions;
using System.Threading;

namespace Harmony
{
    class ActivityClient
    {
        enum ActivityCommandType
        {
            GetCurrent = 0,
            Start = 1
        }

        private readonly XmppClientConnection _xmpp;
        private bool _wait;
        private ActivityCommandType _activityCommand;

        public string CurrentActivity { get; set; }
        public string SessionToken { get; set; }

        public ActivityClient(string ipAddress, int port, string token)
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

        private Document GetStartActivityMessage(string activityId)
        {
            var document = new Document { Namespace = "connect.logitech.com" };

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?startactivity");
            element.Value = string.Format("activityId={0}:timestamp=0", activityId);

            document.AddChild(element);
            return document;
        }

        private Document GetCurrentActivityMessage()
        {
            var document = new Document { Namespace = "connect.logitech.com" };

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?getCurrentActivity");

            document.AddChild(element);
            return document;
        }

        public void StartActivity(string activityId)
        {
            _activityCommand = ActivityCommandType.Start;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(GetStartActivityMessage(activityId));
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

        public void GetCurrentActivity()
        {
            _activityCommand = ActivityCommandType.GetCurrent;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(GetCurrentActivityMessage());
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
                    if (_activityCommand == ActivityCommandType.GetCurrent)
                    {
                        const string identityRegEx = "errorstring=\"OK\">(.*)</oa>";
                        var regex = new Regex(identityRegEx, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        var match = regex.Match(iq.InnerXml);
                        if (match.Success)
                        {
                            CurrentActivity = match.Groups[1].ToString().Split('=')[1];
                        }
                    }

                    _wait = false;
                }
            }
        }
    }
}
