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
	        if (!iq.HasTag("oa")) return;
	        var oaElement = iq.SelectSingleElement("oa");
	        // Keep receiving messages until we get a 200 status
	        // Activity commands send 100 (continue) until they finish
	        if (!"200".Equals(oaElement.GetAttribute("errorcode"))) return;

			var data = oaElement.GetData();
	        foreach (var pair in data.Split(':'))
	        {
		        if (pair.StartsWith("identity"))
		        {
			        _sessionToken = pair.Split('=')[1];
		        }
	        }
	        Wait = false;
        }
    }
}
