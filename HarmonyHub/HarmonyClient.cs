using System;
using System.Diagnostics;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;

namespace HarmonyHub
{
    /// <summary>
    /// Client to interrogate and control Logitech Harmony Hub.
    /// </summary>
    public class HarmonyClient
    {
        protected HarmonyClientConnection Xmpp { get; set; }

        private readonly TaskCompletionSource<bool> _loginTaskCompletionSource = new TaskCompletionSource<bool>();

        /// <summary>
        /// Constructor with standard settings for a new HarmonyClient
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="token"></param>
        public HarmonyClient(string ipAddress, int port, string token)
        {
            Xmpp = new HarmonyClientConnection(ipAddress, port);
            Xmpp.OnLogin += sender => {
                _loginTaskCompletionSource.TrySetResult(true);
            };
            Xmpp.OnSocketError += HandleLoginError;
            Xmpp.Open($"{token}@x.com", token);
        }

        /// <summary>
        /// Help with login errors
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="ex">Exception</param>
        private void HandleLoginError(object sender, Exception ex)
        {
            Debug.WriteLine(ex.Message);
            _loginTaskCompletionSource.TrySetException(ex);
        }

        /// <summary>
        /// This return the login task, and d
        /// </summary>
        /// <returns></returns>
        public Task<bool> LoginAsync()
        {
            return _loginTaskCompletionSource.Task;
        }

        #region Send Messages to HarmonyHub

        /// <summary>
        /// Send message to HarmonyHub to request Configuration.
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        public Task<string> GetConfigAsync()
        {
            var iqToSend = new IQ
            {
                Type = IqType.get,
                Namespace = "",
                From = "1",
                To = "guest"
            };

            iqToSend.AddChild(HarmonyDocuments.ConfigDocument());
            iqToSend.GenerateId();

            // Create a TaskCompletionSource to supply the result to the caller
            var configTaskCompletionSource = new TaskCompletionSource<string>();
            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, (sender, iq, data) =>
            {
                var config = GetData(iq);
                if (config != null)
                {
                    configTaskCompletionSource.TrySetResult(config);
                }
            });
            return configTaskCompletionSource.Task;
        }

        /// <summary>
        /// Send message to HarmonyHub to start a given activity
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="activityId"></param>
        public Task StartActivityAsync(string activityId)
        {
            var iqToSend = new IQ
            {
                Type = IqType.get,
                Namespace = "",
                From = "1",
                To = "guest"
            };
            iqToSend.AddChild(HarmonyDocuments.StartActivityDocument(activityId));
            iqToSend.GenerateId();

            var startActivityTaskCompletionSource = new TaskCompletionSource<bool>();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, (sender, iq, data) =>
            {
                if (iq.Error != null)
                {
                    startActivityTaskCompletionSource.TrySetException(new Exception(iq.Error.ErrorText));
                }
                else
                {
                    startActivityTaskCompletionSource.TrySetResult(true);
                }
            });
            return startActivityTaskCompletionSource.Task;
        }

        /// <summary>
        /// Send message to HarmonyHub to request current activity
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        public Task<string> GetCurrentActivityAsync()
        {
            var iqToSend = new IQ
            {
                Type = IqType.get,
                Namespace = "",
                From = "1",
                To = "guest"
            };
            iqToSend.AddChild(HarmonyDocuments.GetCurrentActivityDocument());
            iqToSend.GenerateId();

            var startActivityTaskCompletionSource = new TaskCompletionSource<string>();
            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, (sender, iq, data) =>
            {
                var currentActivityData = GetData(iq);
                if (currentActivityData != null)
                {
                    var currentActivity = currentActivityData.Split('=')[1];
                    startActivityTaskCompletionSource.TrySetResult(currentActivity);
                }
            });
            return startActivityTaskCompletionSource.Task;
        }

        /// <summary>
        /// Send message to HarmonyHub to request to press a button
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        public void PressButton(string deviceId, string command)
        {
            var iqToSend = new IQ
            {
                Type = IqType.get,
                Namespace = "",
                From = "1",
                To = "guest"
            };
            iqToSend.AddChild(HarmonyDocuments.IrCommandDocument(deviceId, command));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(Xmpp);
            iqGrabber.SendIq(iqToSend, 5);
        }

        /// <summary>
        /// Send message to HarmonyHub to request to turn off all devices
        /// </summary>
        public async Task TurnOffAsync()
        {
            var currentActivity = await GetCurrentActivityAsync().ConfigureAwait(false);
            if (currentActivity != "-1")
            {
                await StartActivityAsync("-1").ConfigureAwait(false);
            }
        }

        #endregion

        /// <summary>
        /// Get the data from the IQ response object
        /// </summary>
        /// <param name="iq">IQ response object</param>
        /// <returns></returns>
        protected string GetData(IQ iq)
        {
            if (iq.HasTag("oa"))
            {
                var oaElement = iq.SelectSingleElement("oa");
                // Keep receiving messages until we get a 200 status
                // Activity commands send 100 (continue) until they finish
                var errorCode = oaElement.GetAttribute("errorcode");
                if ("200".Equals(errorCode))
                {
                    return oaElement.GetData();
                }
                Debug.WriteLine("Error code " + errorCode);
            }
            return null;
        }
    }
}