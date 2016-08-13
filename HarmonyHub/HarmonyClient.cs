using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Sasl;
using agsXMPP.Xml.Dom;
using HarmonyHub.Entities.Response;
using HarmonyHub.Internals;
using HarmonyHub.Utils;

namespace HarmonyHub
{
    /// <summary>
    ///     Client to interrogate and control Logitech Harmony Hub.
    /// </summary>
    public class HarmonyClient : IDisposable
    {
        /// <summary>
        ///     This has the login state..
        ///     When the OnLoginHandler is triggered this is set with true,
        ///     When an error occurs before this, the expeception is set.
        ///     Everywhere where this is awaited the state is returned, but blocks until there is something.
        /// </summary>
        private readonly TaskCompletionSource<bool> _loginTaskCompletionSource = new TaskCompletionSource<bool>();

        // A lookup to correlate request and responses
        private readonly IDictionary<string, TaskCompletionSource<IQ>> _resultTaskCompletionSources = new ConcurrentDictionary<string, TaskCompletionSource<IQ>>();
        // The connection
        private readonly XmppClientConnection _xmpp;

        /// <summary>
        /// This event is triggered when the current activity is changed
        /// </summary>
        public event EventHandler<string> OnActivityChanged;

        /// <summary>
        ///     Constructor with standard settings for a new HarmonyClient
        /// </summary>
        /// <param name="host">IP or hostname</param>
        /// <param name="token">Auth-token, or guest</param>
        /// <param name="port">The port to connect to, default 5222</param>
        private HarmonyClient(string host, string token, int port = 5222)
        {
            Token = token;
            _xmpp = new XmppClientConnection(host, port)
            {
                UseStartTLS = false,
                UseSSL = false,
                UseCompression = false,
                AutoResolveConnectServer = false,
                AutoAgents = false,
                AutoPresence = true,
                AutoRoster = true
            };
            // Configure Sasl not to use auto and PLAIN for authentication
            _xmpp.OnSaslStart += SaslStartHandler;
            _xmpp.OnLogin += OnLoginHandler;
            _xmpp.OnIq += OnIqResponseHandler;
            _xmpp.OnMessage += OnMessage;
            _xmpp.OnSocketError += ErrorHandler;
            // Open the connection, do the login
            _xmpp.Open($"{token}@x.com", token);
        }

        /// <summary>
        ///     Read the token used for the connection, maybe to store it and use it another time.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        ///     Cleanup and close
        /// </summary>
        public void Dispose()
        {
            _xmpp.OnIq -= OnIqResponseHandler;
            _xmpp.OnMessage -= OnMessage;
            _xmpp.OnLogin -= OnLoginHandler;
            _xmpp.OnSocketError -= ErrorHandler;
            _xmpp.OnSaslStart -= SaslStartHandler;
            _xmpp.Close();
        }

        /// <summary>
        ///     Create a HarmonyClient with pre-authenticated token
        /// </summary>
        /// <param name="host">IP or hostname</param>
        /// <param name="token">token which is created via an authentication via myharmony.com</param>
        /// <param name="port">Port to connect to, 5222 is the default</param>
        /// <returns></returns>
        public static HarmonyClient Create(string host, string token, int port = 5222)
        {
            return new HarmonyClient(host, token, port);
        }

        /// <summary>
        ///     Create a harmony client via myharmony.com authenification
        /// </summary>
        /// <param name="host">IP or hostname</param>
        /// <param name="username">myharmony.com username (email)</param>
        /// <param name="password">myharmony.com password</param>
        /// <param name="port">Port to connect to, default 5222</param>
        /// <returns>HarmonyClient</returns>
        public static async Task<HarmonyClient> Create(string host, string username, string password, int port = 5222)
        {
            string userAuthToken = await HarmonyAuthentication.GetUserAuthToken(username, password);
            if (string.IsNullOrEmpty(userAuthToken))
            {
                throw new Exception("Could not get token from Logitech server.");
            }

            // Make a guest connection only to exchange the session token via the user authentication token
            string sessionToken;
            using (var client = new HarmonyClient(host, "guest", port))
            {
                sessionToken = await client.SwapAuthToken(userAuthToken).ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new Exception("Could not swap token on Harmony Hub.");
            }

            // Create the client with the session token
            return new HarmonyClient(host, sessionToken, port);
        }

        /// <summary>
        ///     Send a document, ignore the response (but wait shortly for a possible error)
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="waitTimeout">the time to wait for a possible error, if this is too small errors are ignored.</param>
        /// <returns>Task to await on</returns>
        private async Task FireAndForgetAsync(Document document, int waitTimeout = 50)
        {
            // Check if the login was made, this blocks until there is a state
            // And throws an exception if the login failed.
            await _loginTaskCompletionSource.Task.ConfigureAwait(false);

            // Create the IQ to send
            var iqToSend = GenerateIq(document);

            // Prepate the TaskCompletionSource, which is used to await the result
            var resultTaskCompletionSource = new TaskCompletionSource<IQ>();
            _resultTaskCompletionSources[iqToSend.Id] = resultTaskCompletionSource;

            Debug.WriteLine("Sending (ignoring response):");
            Debug.WriteLine(iqToSend.ToString());
            // Start the sending
            _xmpp.Send(iqToSend);

            // Await, to make sure there wasn't an error
            var task = await Task.WhenAny(resultTaskCompletionSource.Task, Task.Delay(waitTimeout)).ConfigureAwait(false);
            // Make sure the exception, if any, is unwrapped
            await task;
        }

        /// <summary>
        ///     Generate an IQ for the supplied Document
        /// </summary>
        /// <param name="document">Document</param>
        /// <returns>IQ</returns>
        private static IQ GenerateIq(Document document)
        {
            // Create the IQ to send
            var iqToSend = new IQ
            {
                Type = IqType.get,
                Namespace = "",
                From = "1",
                To = "guest"
            };

            // Add the real content for the Harmony
            iqToSend.AddChild(document);

            // Generate an unique ID, this is used to correlate the reply to the request
            iqToSend.GenerateId();
            return iqToSend;
        }

        /// <summary>
        ///     Get the data from the IQ response object
        /// </summary>
        /// <param name="iq">IQ response object</param>
        /// <returns>string with the data of the element</returns>
        private string GetData(IQ iq)
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
            }
            return null;
        }


        /// <summary>
        ///     Send a document, await the response and return it
        /// </summary>
        /// <param name="document">Document</param>
        /// <returns>IQ response</returns>
        private async Task<IQ> RequestResponseAsync(Document document)
        {
            // Check if the login was made, this blocks until there is a state
            // And throws an exception if the login failed.
            await _loginTaskCompletionSource.Task.ConfigureAwait(false);

            // Create the IQ to send
            var iqToSend = GenerateIq(document);

            // Prepate the TaskCompletionSource, which is used to await the result
            var resultTaskCompletionSource = new TaskCompletionSource<IQ>();
            _resultTaskCompletionSources[iqToSend.Id] = resultTaskCompletionSource;

            Debug.WriteLine("Sending:");
            Debug.WriteLine(iqToSend.ToString());
            // Start the sending
            _xmpp.Send(iqToSend);

            // Await / block until an reply arrives or the timeout happens
            return await resultTaskCompletionSource.Task.ConfigureAwait(false);
        }

        #region Authentication

        /// <summary>
        ///     Send message to HarmonyHub with UserAuthToken, wait for SessionToken
        /// </summary>
        /// <param name="userAuthToken"></param>
        /// <returns></returns>
        public async Task<string> SwapAuthToken(string userAuthToken)
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.LogitechPairDocument(userAuthToken)).ConfigureAwait(false);
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

        #region Event Handlers

        /// <summary>
        ///     Handle incomming messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnMessage(object sender, Message message)
        {
            if (!message.HasTag("event"))
            {
                return;
            }
            var eventElement = message.SelectSingleElement("event");
            var eventData = eventElement.GetData();
            if (eventData == null)
            {
                return;
            }
            foreach (var pair in eventData.Split(':'))
            {
                if (!pair.StartsWith("activityId"))
                {
                    continue;
                }
                var activityId = pair.Split('=')[1];
                OnActivityChanged?.Invoke(this, activityId);
            }
        }

        /// <summary>
        ///     Configure Sasl not to use auto and PLAIN for authentication
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="saslEventArgs">SaslEventArgs</param>
        private void SaslStartHandler(object sender, SaslEventArgs saslEventArgs)
        {
            saslEventArgs.Auto = false;
            saslEventArgs.Mechanism = "PLAIN";
        }

        /// <summary>
        ///     Handle login by completing the _loginTaskCompletionSource
        /// </summary>
        /// <param name="sender"></param>
        private void OnLoginHandler(object sender)
        {
            _loginTaskCompletionSource.TrySetResult(true);
        }

        /// <summary>
        ///     Lookup the TaskCompletionSource for the IQ message and try to set the result.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="iq">IQ</param>
        private void OnIqResponseHandler(object sender, IQ iq)
        {
            Debug.WriteLine("Received event " + iq.Id);
            Debug.WriteLine(iq.ToString());
            TaskCompletionSource<IQ> resulTaskCompletionSource;
            if (iq.Id != null && _resultTaskCompletionSources.TryGetValue(iq.Id, out resulTaskCompletionSource))
            {
                _resultTaskCompletionSources.Remove(iq.Id);

                // Error handling from XMPP
                if (iq.Error != null)
                {
                    var errorMessage = iq.Error.ErrorText;
                    Debug.WriteLine(errorMessage);
                    resulTaskCompletionSource.TrySetException(new Exception(errorMessage));
                    return;
                }

                // Message processing (error handling)
                if (iq.HasTag("oa"))
                {
                    var oaElement = iq.SelectSingleElement("oa");

                    // Check error code
                    var errorCode = oaElement.GetAttribute("errorcode");
                    if ("200".Equals(errorCode))
                    {
                        resulTaskCompletionSource.TrySetResult(iq);
                    }
                    else
                    {
                        // We didn't get a 200, this must mean there was an error
                        var errorMessage = oaElement.GetAttribute("errorstring");
                        Debug.WriteLine(errorMessage);
                        // Set the exception on the TaskCompletionSource, it will be picked up in the await
                        resulTaskCompletionSource.TrySetException(new Exception(errorMessage));
                    }
                }
                else
                {
                    Debug.WriteLine("Unexpected content");
                }
            }
            else
            {
                Debug.WriteLine("No matching result task found.");
            }
        }

        /// <summary>
        ///     Help with login errors
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="ex">Exception</param>
        private void ErrorHandler(object sender, Exception ex)
        {
            if (_loginTaskCompletionSource.Task.Status == TaskStatus.Created)
            {
                _loginTaskCompletionSource.TrySetException(ex);
            }
            else
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Send Messages to HarmonyHub

        /// <summary>
        ///     Request the configuration from the hub
        /// </summary>
        /// <returns>HarmonyConfig</returns>
        public async Task<Config> GetConfigAsync()
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.ConfigDocument()).ConfigureAwait(false);
            var config = GetData(iq);
            if (config != null)
            {
                return Serializer.FromJson<Config>(config);
            }
            throw new Exception("No data found");
        }

        /// <summary>
        ///     Send message to HarmonyHub to start a given activity
        ///     Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="activityId">string</param>
        public async Task StartActivityAsync(string activityId)
        {
            await RequestResponseAsync(HarmonyDocuments.StartActivityDocument(activityId)).ConfigureAwait(false);
        }

        /// <summary>
        ///     Send message to HarmonyHub to request current activity
        ///     Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <returns>string with the current activity</returns>
        public async Task<string> GetCurrentActivityAsync()
        {
            var iq = await RequestResponseAsync(HarmonyDocuments.GetCurrentActivityDocument()).ConfigureAwait(false);
            var currentActivityData = GetData(iq);
            if (currentActivityData != null)
            {
                return currentActivityData.Split('=')[1];
            }
            throw new Exception("No data found in IQ");
        }

        /// <summary>
        ///     Send message to HarmonyHub to request to press a button
        ///     Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="deviceId">string with the ID of the device</param>
        /// <param name="command">string with the command for the device</param>
        /// <param name="press">true for press, false for release</param>
        /// <param name="timestamp">Timestamp for the command, e.g. send a press with 0 and a release with 100</param>
        public async Task SendCommandAsync(string deviceId, string command, bool press = true, int timestamp = 0)
        {
            var document = HarmonyDocuments.IrCommandDocument(deviceId, command, press, timestamp);
            await FireAndForgetAsync(document).ConfigureAwait(false);
        }

        /// <summary>
        ///     Send a message that a button was pressed
        ///     Result is parsed by OnIq based on ClientCommandType
        /// </summary>
        /// <param name="deviceId">string with the ID of the device</param>
        /// <param name="command">string with the command for the device</param>
        /// <param name="timespan">The time between the press and release, default 100ms</param>
        public async Task SendKeyPressAsync(string deviceId, string command, int timespan = 100)
        {
            var press = HarmonyDocuments.IrCommandDocument(deviceId, command);
            var release = HarmonyDocuments.IrCommandDocument(deviceId, command, false, timespan);
            await FireAndForgetAsync(press).ConfigureAwait(false);
            await FireAndForgetAsync(release).ConfigureAwait(false);
        }

        /// <summary>
        ///     Send message to HarmonyHub to request to turn off all devices
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
    }
}