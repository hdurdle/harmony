using System;
using System.Web.Script.Serialization;
using agsXMPP.Xml.Dom;

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

        public static HarmonyDocument GetStartActivityMessage(string activityId)
        {
            var document = new HarmonyDocument();

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?startactivity");
            element.Value = string.Format("activityId={0}:timestamp=0", activityId);

            document.AddChild(element);
            return document;
        }

        public static HarmonyDocument GetCurrentActivityMessage()
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
            // "action":"{\"command\":\"VolumeDown\",\"type\":\"IRCommand\",\"deviceId\":\"14766260\"}",
            element.Value = string.Format("action={0}:status=press", json);

            Console.WriteLine(element.Value);

            document.AddChild(element);
            return document;
        }

        public static HarmonyDocument GetConfigMessage()
        {
            var document = new HarmonyDocument();

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.harmony/vnd.logitech.harmony.engine?config");

            document.AddChild(element);
            return document;
        }

        public static HarmonyDocument GetAuthMessage(string token)
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
