using agsXMPP.Xml.Dom;
using System.Web.Script.Serialization;

namespace HarmonyHub
{
    class HarmonyDocuments
    {
        public class HarmonyDocument : Document
        {
            public HarmonyDocument()
            {
                Namespace = "connect.logitech.com";
            }
        }

        public static HarmonyDocument StartActivityDocument(string activityId)
        {
            var document = new HarmonyDocument();

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?startactivity");
            element.Value = string.Format("activityId={0}:timestamp=0", activityId);

            document.AddChild(element);
            return document;
        }

        public static HarmonyDocument GetCurrentActivityDocument()
        {
            var document = new HarmonyDocument();

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?getCurrentActivity");

            document.AddChild(element);
            return document;
        }

        public static HarmonyDocument IRCommandDocument(string deviceId, string command)
        {
            var document = new HarmonyDocument();

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?holdAction");

            var action = new HarmonyAction { type = "IRCommand", deviceId = deviceId, command = command };
            var json = new JavaScriptSerializer().Serialize(action);

            // At this point our valid json won't work - we need to break it so it looks like:
            // {"type"::"IRCommand","deviceId"::"deviceId","command"::"command"}
            // note double colons 

            json = json.Replace(":", "::");

            element.Value = string.Format("action={0}:status=press", json);

            document.AddChild(element);

            return document;
        }

        public static HarmonyDocument ConfigDocument()
        {
            var document = new HarmonyDocument();

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?config");

            document.AddChild(element);
            return document;
        }

        public static HarmonyDocument LogitechPairDocument(string token)
        {
            var document = new HarmonyDocument();

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.connect/vnd.logitech.pair");
            element.Value = string.Format("token={0}:name={1}#{2}", token, "foo", "iOS6.0.1#iPhone");

            document.AddChild(element);
            return document;
        }

    }
}
