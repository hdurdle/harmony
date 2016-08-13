using System.Web.Script.Serialization;
using agsXMPP.Xml.Dom;
using HarmonyHub.Entities;
using HarmonyHub.Utils;

namespace HarmonyHub
{
    internal class HarmonyDocuments
    {
        private const string Namespace = "connect.logitech.com";


        private static Element CreateOaElement(string command)
        {
            var element = new Element("oa");
            element.Attributes.Add("xmlns", Namespace);
            element.Attributes.Add("mime", $"vnd.logitech.harmony/vnd.logitech.harmony.engine?{command}");
            return element;
        }

        public static Document StartActivityDocument(string activityId)
        {
            var document = new Document
            {
                Namespace = Namespace
            };

            var element = CreateOaElement("startactivity");
            element.Value = $"activityId={activityId}:timestamp=0";
            document.AddChild(element);
            return document;
        }

        public static Document GetCurrentActivityDocument()
        {
            var document = new Document
            {
                Namespace = Namespace
            };
            document.AddChild(CreateOaElement("getCurrentActivity"));
            return document;
        }

        public static Document IrCommandDocument(string deviceId, string command)
        {
            var document = new Document
            {
                Namespace = Namespace
            };

            var action = new HarmonyAction
            {
                Type = "IRCommand",
                DeviceId = deviceId,
                Command = command
            };
			var json = Serializer.ToJson<HarmonyAction>(action);

            // At this point our valid json won't work - we need to break it so it looks like:
            // {"type"::"IRCommand","deviceId"::"deviceId","command"::"command"}
            // note double colons 

            json = json.Replace(":", "::");

            var element = CreateOaElement("holdAction");

            element.Value = $"action={json}:status=press";

            document.AddChild(element);

            return document;
        }

        public static Document ConfigDocument()
        {
            var document = new Document
            {
                Namespace = Namespace
            };
            document.AddChild(CreateOaElement("config"));
            return document;
        }

        public static Document LogitechPairDocument(string token)
        {
            var document = new Document
            {
                Namespace = Namespace
            };

            var element = new Element("oa");
            element.Attributes.Add("xmlns", "connect.logitech.com");
            element.Attributes.Add("mime", "vnd.logitech.connect/vnd.logitech.pair");
            element.Value = $"token={token}:name=foo#iOS6.0.1#iPhone";
            document.AddChild(element);
            return document;
        }
    }
}