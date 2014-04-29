using agsXMPP;
using agsXMPP.protocol.client;
using System.Text.RegularExpressions;
using System.Threading;

namespace HarmonyHub
{
    public class HarmonyClient
    {
        protected bool Wait;

        protected XmppClientConnection Xmpp;

        enum ClientCommandType
        {
            GetCurrentActivity = 0,
            StartActivity = 1,
            PressButton = 2,
            GetConfig = 3
        }

        private ClientCommandType _clientCommand;

        public string Config { get; set; }
        public string SessionToken { get; set; }
        public string CurrentActivity { get; set; }

        public HarmonyClient(string ipAddress, int port, string token)
        {
            Xmpp = new HarmonyClientConnection(ipAddress, port);
            Xmpp.OnLogin += delegate { Wait = false; };

            SessionToken = token;
            string username = string.Format("{0}@x.com", token);

            Xmpp.OnIq += OnIq;
            Xmpp.Open(username, token);

            WaitForData(5);
        }

        public void GetConfig()
        {
            _clientCommand = ClientCommandType.GetConfig;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(HarmonyDocuments.ConfigDocument());
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        public void StartActivity(string activityId)
        {
            _clientCommand = ClientCommandType.StartActivity;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(HarmonyDocuments.StartActivityDocument(activityId));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        public void GetCurrentActivity()
        {
            _clientCommand = ClientCommandType.GetCurrentActivity;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(HarmonyDocuments.GetCurrentActivityDocument());
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        public void PressButton(string deviceId, string command)
        {
            _clientCommand = ClientCommandType.PressButton;

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(HarmonyDocuments.IRCommandDocument(deviceId, command));
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

        void OnIq(object sender, IQ iq)
        {
            if (iq.HasTag("oa"))
            {
                if (iq.InnerXml.Contains("errorcode=\"200\""))
                {
                    const string identityRegEx = "errorstring=\"OK\">(.*)</oa>";
                    var regex = new Regex(identityRegEx, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (_clientCommand == ClientCommandType.PressButton)
                    {
                        //Console.WriteLine(iq.InnerXml);
                    }

                    if (_clientCommand == ClientCommandType.GetConfig)
                    {
                        var match = regex.Match(iq.InnerXml);
                        if (match.Success)
                        {
                            Config = match.Groups[1].ToString();
                        }
                    }

                    if (_clientCommand == ClientCommandType.GetCurrentActivity)
                    {
                        var match = regex.Match(iq.InnerXml);
                        if (match.Success)
                        {
                            CurrentActivity = match.Groups[1].ToString().Split('=')[1];
                        }
                    }


                    if (_clientCommand == ClientCommandType.StartActivity)
                    {

                    }

                    Wait = false;
                }
            }
        }

    }
}
