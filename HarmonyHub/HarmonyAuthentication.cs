using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HarmonyHub.Entities.Auth;
using HarmonyHub.Utils;

namespace HarmonyHub
{
    /// <summary>
    /// This class handles the Http communication with Logitech
    /// </summary>
    public static class HarmonyAuthentication
    {
        /// <summary>
        /// This is the Url to the logitech service myharmony, which can get the user authentication for Harmony-hubs.
        /// </summary>
        public const string LogitechAuthUrl = "https://svcs.myharmony.com/CompositeSecurityServices/Security.svc/json/GetUserAuthToken";

        /// <summary>
        ///     Logs in to the Logitech Harmony web service to get a UserAuthToken.
        /// </summary>
        /// <param name="username">myharmony.com username</param>
        /// <param name="password">myharmony.com password</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Logitech UserAuthToken</returns>
        public static async Task<string> GetUserAuthToken(string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the default proxy, and configure it
            var proxyToUse = WebRequest.GetSystemWebProxy();
            if (proxyToUse is WebProxy)
            {
                // Read note here: https://msdn.microsoft.com/en-us/library/system.net.webproxy.credentials.aspx
                var webProxy = proxyToUse as WebProxy;
                webProxy.UseDefaultCredentials = true;
            }
            else
            {
                proxyToUse.Credentials = CredentialCache.DefaultCredentials;
            }

            // Create a HttpClientHandler, and a HttpClient, in a using so they are disposed
            using (var httpClientHandler = new HttpClientHandler() { UseProxy = true, Proxy = proxyToUse})
            using (var httpClient = new HttpClient(httpClientHandler))
            {
                // Configure the httpClient for json and don't expect a continue
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Prepare the json string
                var credentialsJson = Serializer.ToJson(new Credentials
                {
                    Username = username,
                    Password = password
                });
                // Create a HttpContent for the string
                var jsonContent = new StringContent(credentialsJson, Encoding.UTF8);

                // Post and get the reponse message
                var httpResponseMessage = await httpClient.PostAsync(new Uri(LogitechAuthUrl), jsonContent, cancellationToken);

                // Ensure we got a succes, if not this will throw
                httpResponseMessage.EnsureSuccessStatusCode();

                // Get the result
                var result = await httpResponseMessage.Content.ReadAsStringAsync();

                // Deserialize the result
                var harmonyData = Serializer.FromJson<GetUserAuthTokenResultRootObject>(result);

                // Return the part that we need
                return harmonyData.GetUserAuthTokenResult.UserAuthToken;
            }
        }
    }
}
