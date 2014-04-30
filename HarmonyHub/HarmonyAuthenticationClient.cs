using agsXMPP;
using agsXMPP.protocol.client;
using System.Text.RegularExpressions;

namespace HarmonyHub
{
    /// <summary>
    /// HarmonyClient for swapping a UserAuthToken for a Session Token
    /// </summary>
    public class HarmonyAuthenticationClient : HarmonyClient
    {
        private string _sessionToken;

        public HarmonyAuthenticationClient(string ipAddress, int port)
            : base(ipAddress, port, "guest")
        {
            Xmpp.OnIq += OnIq;
        }

        /// <summary>
        /// Send message to HarmonyHub with UserAuthToken, wait for SessionToken
        /// </summary>
        /// <param name="userAuthToken"></param>
        /// <returns></returns>
        public string SwapAuthToken(string userAuthToken)
        {
            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(HarmonyDocuments.LogitechPairDocument(userAuthToken));
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
