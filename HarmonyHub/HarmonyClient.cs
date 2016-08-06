using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using agsXMPP;
using agsXMPP.protocol.client;
using HarmonyHub.Entities;

namespace HarmonyHub
{
    /// <summary>
    /// Client to interrogate and control Logitech Harmony Hub.
    /// </summary>
    public class HarmonyClient : IDisposable
    {
        private readonly HarmonyClientConnection _xmpp;
        private readonly IDictionary<string, TaskCompletionSource<IQ>> _resultTaskCompletionSources = new ConcurrentDictionary<string, TaskCompletionSource<IQ>>();
        private readonly TaskCompletionSource<bool> _loginTaskCompletionSource = new TaskCompletionSource<bool>();

        /// <summary>
        /// Constructor with standard settings for a new HarmonyClient
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="token"></param>
        public HarmonyClient(string ipAddress, int port, string token)
        {
            _xmpp = new HarmonyClientConnection(ipAddress, port);
            _xmpp.OnLogin += OnLogin;
            _xmpp.OnIq += OnIqResponse;
            _xmpp.OnSocketError += HandleLoginError;
            _xmpp.Open($"{token}@x.com", token);
        }

        /// <summary>
        /// Sennd a document, await the response and return it
        /// </summary>
        /// <param name="document"></param>
        /// <returns>IQ response</returns>
        private async Task<IQ> RequestResponseAsync(HarmonyDocument document)
        {
            await _loginTaskCompletionSource.Task.ConfigureAwait(false);
            var iqToSend = new IQ
            {
                Type = IqType.get,
                Namespace = "",
                From = "1",
                To = "guest"
            };

            iqToSend.AddChild(document);
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(_xmpp);
            var resultTaskCompletionSource = new TaskCompletionSource<IQ>();

            Debug.WriteLine("Storing callback for " + iqToSend.Id);
            _resultTaskCompletionSources[iqToSend.Id] = resultTaskCompletionSource;
            iqGrabber.SendIq(iqToSend, 1000);
            return await resultTaskCompletionSource.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// Handle login by completing the _loginTaskCompletionSource
        /// </summary>
        /// <param name="sender"></param>
        private void OnLogin(object sender)
        {
            Debug.WriteLine("Login success.");
            _loginTaskCompletionSource.TrySetResult(true);
        }

        /// <summary>
        /// Lookup the TaskCompletionSource for the IQ message and try to set the result.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="iq">IQ</param>
        private void OnIqResponse(object sender, IQ iq)
        {
            Debug.WriteLine("Received event " + iq.Id);
            TaskCompletionSource<IQ> resulTaskCompletionSource;
            if (iq.Id != null && _resultTaskCompletionSources.TryGetValue(iq.Id, out resulTaskCompletionSource))
            {
                _resultTaskCompletionSources.Remove(iq.Id);
                resulTaskCompletionSource.TrySetResult(iq);
            }
            else
            {
                Debug.WriteLine("No result task found.");
            }
        }

        /// <summary>
        /// Cleanup and close
        /// </summary>
        public void Dispose()
        {
            _xmpp.OnIq -= OnIqResponse;
            _xmpp.OnLogin -= OnLogin;
            _xmpp.OnSocketError -= HandleLoginError;
            _xmpp.Close();
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

        #region Authentication
        /// <summary>
        /// Send message to HarmonyHub with UserAuthToken, wait for SessionToken
        /// </summary>
        /// <param name="userAuthToken"></param>
        /// <returns></returns>
        public async Task<string> SwapAuthToken(string userAuthToken)
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.LogitechPairDocument(userAuthToken));
            var sessionData = GetData(iq);
            if (sessionData != null)
            {
                foreach (var pair in sessionData.Split(':'))
                {
                    if (pair.StartsWith("identity"))
                    {
                        return pair.Split('=')[1];
                    }
                }
            }
            throw new Exception("Wrong data");
        }
        #endregion


        #region Send Messages to HarmonyHub

        /// <summary>
        /// Send message to HarmonyHub to request Configuration.
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        public async Task<HarmonyConfigResult> GetConfigAsync()
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.ConfigDocument());
            var config = GetData(iq);
            if (config != null)
            {
                return new JavaScriptSerializer().Deserialize<HarmonyConfigResult>(config);
            }
            throw new Exception("Wrong data");
        }

        /// <summary>
        /// Send message to HarmonyHub to start a given activity
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="activityId"></param>
        public async Task StartActivityAsync(string activityId)
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.StartActivityDocument(activityId));
            if (iq.Error != null)
            {
                throw new Exception(iq.Error.ErrorText);
            }
        }

        /// <summary>
        /// Send message to HarmonyHub to request current activity
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        public async Task<string> GetCurrentActivityAsync()
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.GetCurrentActivityDocument());
            var currentActivityData = GetData(iq);
            if (currentActivityData != null)
            {
                return currentActivityData.Split('=')[1];
            }
            throw new Exception("Wrong data");
        }

        /// <summary>
        /// Send message to HarmonyHub to request to press a button
        /// Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        public async Task PressButtonAsync(string deviceId, string command)
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.IrCommandDocument(deviceId, command));
            if (iq.Error != null)
            {
                throw new Exception(iq.Error.ErrorText);
            }
            throw new Exception("Wrong data");
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
            Debug.WriteLine("GetData: Called " + iq.Id);
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