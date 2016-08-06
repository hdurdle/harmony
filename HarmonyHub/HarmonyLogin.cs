using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using HarmonyHub.Entities;

namespace HarmonyHub
{
    public class HarmonyLogin
    {
        public static async Task<string> LoginToLogitechAsync(string email, string password, string ipAddress, int harmonyPort)
        {
            string userAuthToken = GetUserAuthToken(email, password);
            if (string.IsNullOrEmpty(userAuthToken))
            {
                throw new Exception("Could not get token from Logitech server.");
            }

            string sessionToken;

            using (var client = new HarmonyClient(ipAddress, harmonyPort, "guest"))
            {
                sessionToken = await client.SwapAuthToken(userAuthToken).ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new Exception("Could not swap token on Harmony Hub.");
            }

            File.WriteAllText("SessionToken", sessionToken);

            Console.WriteLine($"Date Time : {DateTime.Now}");
            Console.WriteLine($"User Token: {userAuthToken}");
            Console.WriteLine($"Sess Token: {sessionToken}");

            return sessionToken;
        }

        /// <summary>
        /// Logs in to the Logitech Harmony web service to get a UserAuthToken.
        /// </summary>
        /// <param name="username">myharmony.com username</param>
        /// <param name="password">myharmony.com password</param>
        /// <returns>Logitech UserAuthToken</returns>
        public static string GetUserAuthToken(string username, string password)
        {
            const string logitechAuthUrl = "https://svcs.myharmony.com/CompositeSecurityServices/Security.svc/json/GetUserAuthToken";

            var httpWebRequest = (HttpWebRequest) WebRequest.Create(logitechAuthUrl);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = new JavaScriptSerializer().Serialize(new
                {
                    email = username,
                    password
                });

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            var responseStream = httpResponse.GetResponseStream();
            if (responseStream == null) return null;

            using (var streamReader = new StreamReader(responseStream))
            {
                var result = streamReader.ReadToEnd();
                var harmonyData = new JavaScriptSerializer().Deserialize<GetUserAuthTokenResultRootObject>(result);
                return harmonyData.GetUserAuthTokenResult.UserAuthToken;
            }
        }
    }
}