using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace Harmony
{
    public class HarmonyAuthentication
    {
        const string LogitechAuthUrl = "https://svcs.myharmony.com/CompositeSecurityServices/Security.svc/json/GetUserAuthToken";

        private const string HubUsername = "guest@x.com";
        private const string HubPassword = "guest";

        private XmppClientConnection _xmpp;

        private string _sessionToken;
        private bool _wait;

        public string Login(string username, string password)
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
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                // ReSharper disable AssignNullToNotNullAttribute
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                // ReSharper restore AssignNullToNotNullAttribute
                {
                    var result = streamReader.ReadToEnd();
                    var harmonyData = new JavaScriptSerializer().Deserialize<RootObject>(result);
                    return harmonyData.GetUserAuthTokenResult.UserAuthToken;
                }
            }
        }

        private Document GetAuthMessage(string token)
        {
            var document = new Document { Namespace = "connect.logitech.com" };

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.connect/vnd.logitech.pair");
            element.Value = string.Format("token={0}:name={1}#{2}", token, "1vm7ATw/tN6HXGpQcCs/A5MkuvI", "iOS6.0.1#iPhone");

            document.AddChild(element);
            return document;
        }

        public string SwapAuthToken(string ipAddress, int port, string token)
        {
            _xmpp = new HarmonyClientConnection(ipAddress, port);
            _xmpp.OnLogin += delegate { _wait = false; };
            _xmpp.OnIq += OnIq;
            _xmpp.Open(HubUsername, HubPassword);

            _wait = true;
            int i = 0;
            do
            {
                i++;
                if (i == 20)
                    _wait = false;
                Thread.Sleep(500);
            } while (_wait);

            var iqToSend = new IQ { Type = IqType.get, Namespace = "", From = "1", To = "guest" };
            iqToSend.AddChild(GetAuthMessage(token));
            iqToSend.GenerateId();

            var iqGrabber = new IqGrabber(_xmpp);
            iqGrabber.SendIq(iqToSend, 10);

            _wait = true;
            i = 0;
            do
            {
                i++;
                if (i == 20)
                    _wait = false;
                Thread.Sleep(500);
            } while (_wait);

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
                }
            }
            _wait = false;
        }
    }
}
