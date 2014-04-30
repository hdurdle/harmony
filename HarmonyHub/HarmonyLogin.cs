using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace HarmonyHub
{
    public class HarmonyLogin
    {
        /// <summary>
        /// Logs in to the Logitech Harmony web service to get a UserAuthToken.
        /// </summary>
        /// <param name="username">myharmony.com username</param>
        /// <param name="password">myharmony.com password</param>
        /// <returns>Logitech UserAuthToken</returns>
        public static string GetUserAuthToken(string username, string password)
        {
            const string logitechAuthUrl = "https://svcs.myharmony.com/CompositeSecurityServices/Security.svc/json/GetUserAuthToken";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(logitechAuthUrl);
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

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

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
