using System.Threading;
using agsXMPP;
using agsXMPP.protocol.client;

namespace HarmonyHub
{
    /// <summary>
    /// Client to interrogate and control Logitech Harmony Hub.
    /// </summary>
    public class HarmonyClient
    {
        protected bool Wait;

        protected HarmonyClientConnection Xmpp;

        enum ClientCommandType
        {
            None = -1,
            GetCurrentActivity = 0,
            StartActivity = 1,
            PressButton = 2,
            GetConfig = 3
        }

        private ClientCommandType _clientCommand = ClientCommandType.None;

        public string Config { get; set; }
        public string SessionToken { get; set; }
        public string CurrentActivity { get; set; }

        /// <summary>
        /// Constructor with standard settings for a new HarmonyClient
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="token"></param>
        public HarmonyClient(string ipAddress, int port, string token)
        {
            Xmpp = new HarmonyClientConnection(ipAddress, port);
            Xmpp.OnLogin += delegate { Wait = false; };

            SessionToken = token;
            string username = $"{token}@x.com";

            Xmpp.OnIq += OnIq;
            Xmpp.Open(username, token);

            WaitForData(5);
        }

        #region Send Messages to HarmonyHub

        /// <summary>
        /// Send message to HarmonyHub to request Configuration.
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        public void GetConfig()
        {
            _clientCommand = ClientCommandType.GetConfig;

            var iqToSend = new IQ {Type = IqType.get, Namespace = "", From = "1", To = "guest"};
            iqToSend.AddChild(HarmonyDocuments.ConfigDocument());
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        /// <summary>
        /// Send message to HarmonyHub to start a given activity
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="activityId"></param>
        public void StartActivity(string activityId)
        {
            _clientCommand = ClientCommandType.StartActivity;

            var iqToSend = new IQ {Type = IqType.get, Namespace = "", From = "1", To = "guest"};
            iqToSend.AddChild(HarmonyDocuments.StartActivityDocument(activityId));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        /// <summary>
        /// Send message to HarmonyHub to request current activity
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        public void GetCurrentActivity()
        {
            _clientCommand = ClientCommandType.GetCurrentActivity;

            var iqToSend = new IQ {Type = IqType.get, Namespace = "", From = "1", To = "guest"};
            iqToSend.AddChild(HarmonyDocuments.GetCurrentActivityDocument());
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            WaitForData(5);
        }

        /// <summary>
        /// Send message to HarmonyHub to request to press a button
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        public void PressButton(string deviceId, string command)
        {
            _clientCommand = ClientCommandType.PressButton;

            var iqToSend = new IQ {Type = IqType.get, Namespace = "", From = "1", To = "guest"};
            iqToSend.AddChild(HarmonyDocuments.IrCommandDocument(deviceId, command));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 5);

            WaitForData(5);
        }

        /// <summary>
        /// Send message to HarmonyHub to request to turn off all devices
        /// </summary>
        public void TurnOff()
        {
            GetCurrentActivity();
            if (CurrentActivity != "-1")
            {
                StartActivity("-1");
            }
        }

        #endregion

        /// <summary>
        /// Wait for timeoutSeconds to allow messages to be received from the HarmonyHub
        /// </summary>
        /// <param name="timeoutSeconds"></param>
        protected void WaitForData(int timeoutSeconds)
        {
            Wait = true;
            int i = 0;
            do
            {
                i++;
                if (i == (timeoutSeconds*2))
                    Wait = false;
                Thread.Sleep(500);
            } while (Wait);
        }

        /// <summary>
        /// Fires on receipt of an Iq message from the HarmonyHub
        /// Check ClientCommandType to determine what to do
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="iq"></param>
        void OnIq(object sender, IQ iq)
        {
            if (iq.HasTag("oa"))
            {
                var oaElement = iq.SelectSingleElement("oa");
                // Keep receiving messages until we get a 200 status
                // Activity commands send 100 (continue) until they finish
                if ("200".Equals(oaElement.GetAttribute("errorcode")))
                {
                    var data = oaElement.GetData();
                    switch (_clientCommand)
                    {
                        case ClientCommandType.GetConfig:
                            Config = data;
                            break;
                        case ClientCommandType.GetCurrentActivity:
                            // result=<activity>
                            CurrentActivity = data.Split('=')[1];
                            break;
                        case ClientCommandType.PressButton:
                            break;
                        case ClientCommandType.StartActivity:
                            break;
                    }

                    Wait = false;
                }
            }
        }
    }
}