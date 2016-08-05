using agsXMPP;
using agsXMPP.protocol.client;
using System.Threading.Tasks;

namespace HarmonyHub
{
    /// <summary>
    /// HarmonyClient for swapping a UserAuthToken for a Session Token
    /// </summary>
    public class HarmonyAuthenticationClient : HarmonyClient
    {
        public HarmonyAuthenticationClient(string ipAddress, int port)
            : base(ipAddress, port, "guest")
        {
        }

        /// <summary>
        /// Send message to HarmonyHub with UserAuthToken, wait for SessionToken
        /// </summary>
        /// <param name="userAuthToken"></param>
        /// <returns></returns>
        public Task<string> SwapAuthToken(string userAuthToken)
        {
            var iqToSend = new IQ
            {
                Type = IqType.get,
                Namespace = "",
                From = "1",
                To = "guest"
            };
            iqToSend.AddChild(HarmonyDocuments.LogitechPairDocument(userAuthToken));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            var taskCompletionSource = new TaskCompletionSource<string>();

            iqGrabber.SendIq(iqToSend, (sender, iq, data) =>
            {
                var sessionData = GetData(iq);
                if (sessionData != null)
                {
                    foreach (var pair in sessionData.Split(':'))
                    {
                        if (pair.StartsWith("identity"))
                        {
                            var sessionToken = pair.Split('=')[1];
                            taskCompletionSource.TrySetResult(sessionToken);
                        }
                    }
                }
            });

            return taskCompletionSource.Task;
        }
    }
}