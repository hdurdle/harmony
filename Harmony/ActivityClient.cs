using System;
using System.Web.Script.Serialization;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System.Text.RegularExpressions;

namespace Harmony
{
    class ActivityClient : HarmonyClient
    {
        enum ActivityCommandType
        {
            GetCurrent = 0,
            Start = 1,
            PressButton = 2
        }

        private ActivityCommandType _activityCommand;

        public string CurrentActivity { get; set; }
        public string SessionToken { get; set; }

        public ActivityClient(string ipAddress, int port, string token)
            : base(ipAddress, port)
        {
            SessionToken = token;
            string username = string.Format("{0}@x.com", token);

            Xmpp.OnIq += OnIq;
            Xmpp.Open(username, token);

            WaitForData(5);
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

        private Document IRCommandDocument(string deviceId, string command)
        {
            var document = new Document { Namespace = "connect.logitech.com" };

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?holdAction");

            var action = new HarmonyAction { type = "IRCommand", deviceId = deviceId, command = command };
            var json = new JavaScriptSerializer().Serialize(action);
            // "action":"{\"command\":\"VolumeDown\",\"type\":\"IRCommand\",\"deviceId\":\"14766260\"}",
            element.Value = string.Format("action={0}:status=press", json);

            Console.WriteLine(element.Value);

            document.AddChild(element);
            return document;
        }

        public void StartActivity(string activityId)
        {
            _activityCommand = ActivityCommandType.Start;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(GetStartActivityMessage(activityId));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        public void GetCurrentActivity()
        {
            _activityCommand = ActivityCommandType.GetCurrent;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(GetCurrentActivityMessage());
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        public void PressButton(string deviceId, string command)
        {
            _activityCommand = ActivityCommandType.PressButton;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(IRCommandDocument(deviceId, command));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        public void TurnOff()
        {
            GetCurrentActivity();
            if (CurrentActivity != "-1")
            {
                StartActivity("-1");
            }
        }

        void OnIq(object sender, IQ iq)
        {
            if (_activityCommand == ActivityCommandType.PressButton)
            {
                Console.WriteLine(iq.InnerXml);
            }

            if (iq.HasTag("oa"))
            {
                if (iq.InnerXml.Contains("errorcode=\"200\""))
                {
                    const string identityRegEx = "errorstring=\"OK\">(.*)</oa>";
                    var regex = new Regex(identityRegEx, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (_activityCommand == ActivityCommandType.GetCurrent)
                    {
                        var match = regex.Match(iq.InnerXml);
                        if (match.Success)
                        {
                            CurrentActivity = match.Groups[1].ToString().Split('=')[1];
                        }
                    }


                    if (_activityCommand == ActivityCommandType.Start)
                    {

                    }

                    Wait = false;
                }
            }
        }
    }
}
