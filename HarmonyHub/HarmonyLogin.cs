using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace HarmonyHub
{
    public class HarmonyLogin
    {
        private const string LogitechAuthUrl = "https://svcs.myharmony.com/CompositeSecurityServices/Security.svc/json/GetUserAuthToken";

        public static string Login(string username, string password)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(LogitechAuthUrl);
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

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var harmonyData = new JavaScriptSerializer().Deserialize<GetUserAuthTokenResultRootObject>(result);
                return harmonyData.GetUserAuthTokenResult.UserAuthToken;
            }
        }
    }
}
