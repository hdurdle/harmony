using agsXMPP.Xml.Dom;
using HarmonyHub.Entities;
using HarmonyHub.Utils;

namespace HarmonyHub
{
	/// <summary>
	/// Internally used to create different documents to send to the harmony hub.
	/// </summary>
    internal class HarmonyDocuments
    {
        private const string Namespace = "connect.logitech.com";

		/// <summary>
		/// Create the base oa element for harmony documents
		/// </summary>
		/// <param name="command">Command to call</param>
		/// <returns>Element</returns>
        private static Element CreateOaElement(string command)
        {
            var element = new Element("oa");
            element.Attributes.Add("xmlns", Namespace);
            element.Attributes.Add("mime", $"vnd.logitech.harmony/vnd.logitech.harmony.engine?{command}");
            return element;
        }

		/// <summary>
		/// Create a document to start an activity
		/// </summary>
		/// <param name="activityId">Id for the activity</param>
		/// <returns>Document</returns>
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

		/// <summary>
		/// Create a document to request the current activity to the harmony hub
		/// </summary>
		/// <returns>Document</returns>
        public static Document GetCurrentActivityDocument()
        {
            var document = new Document
            {
                Namespace = Namespace
            };
            document.AddChild(CreateOaElement("getCurrentActivity"));
            return document;
        }

	    /// <summary>
	    /// Create a document 
	    /// </summary>
	    /// <param name="deviceId"></param>
	    /// <param name="command"></param>
	    /// <param name="press">true for press, false for release</param>
	    /// <param name="timestamp"></param>
	    /// <returns>Document for the command to send to the harmony hub.</returns>
	    public static Document IrCommandDocument(string deviceId, string command, bool press = true, int timestamp = 0)
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
			var json = Serializer.ToJson(action);

            // At this point our valid json won't work - we need to break it so it looks like:
            // {"type"::"IRCommand","deviceId"::"deviceId","command"::"command"}
            // note double colons 

            json = json.Replace(":", "::");

            var element = CreateOaElement("holdAction");

            element.Value = $"action={json}:status={(press ? "press" : "release")}:timestamp={timestamp}";

            document.AddChild(element);

            return document;
        }

		/// <summary>
		/// Create a document to request the current configuration
		/// </summary>
		/// <returns>Document</returns>
        public static Document ConfigDocument()
        {
            var document = new Document
            {
                Namespace = Namespace
            };
            document.AddChild(CreateOaElement("config"));
            return document;
        }

		/// <summary>
		/// Create a "pair" document
		/// </summary>
		/// <param name="token">Token</param>
		/// <returns>Document</returns>
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