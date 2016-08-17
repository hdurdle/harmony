using System.Text;
using agsXMPP.Xml.Dom;
using HarmonyHub.Entities.Request;
using HarmonyHub.Utils;

namespace HarmonyHub.Internals
{
    /// <summary>
    ///     Internally used to create different documents to send to the harmony hub.
    /// </summary>
    internal class HarmonyDocuments
    {
        private const string Namespace = "connect.logitech.com";

        /// <summary>
        ///     Create a document to request the current configuration
        /// </summary>
        /// <returns>Document</returns>
        public static Document ConfigDocument()
        {
            return CreateDocument(HarmonyCommands.config);
        }

        /// <summary>
        ///     Create a simple document for the command
        /// </summary>
        /// <param name="command">Command to call</param>
        /// <param name="elementValue">The value of the OA element, if one is needed</param>
        /// <returns>Document</returns>
        private static Document CreateDocument(HarmonyCommands command, string elementValue = null)
        {
            var document = new Document
            {
                Namespace = Namespace
            };

            var element = CreateOaElement(command);
            if (elementValue != null)
            {
                element.Value = elementValue;
            }
            document.AddChild(element);
            return document;
        }

        /// <summary>
        ///     Create the base oa element for harmony documents
        /// </summary>
        /// <param name="command">Command to call</param>
        /// <returns>Element</returns>
        private static Element CreateOaElement(HarmonyCommands command)
        {
            var element = new Element("oa");
            element.Attributes.Add("xmlns", Namespace);
            element.Attributes.Add("mime", $"vnd.logitech.harmony/vnd.logitech.harmony.engine?{command}");
            return element;
        }

        /// <summary>
        ///     Create a document to request the current activity to the harmony hub
        /// </summary>
        /// <returns>Document</returns>
        public static Document GetCurrentActivityDocument()
        {
            return CreateDocument(HarmonyCommands.getCurrentActivity);
        }

        /// <summary>
        ///     Create a document
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        /// <param name="press">true for press, false for release</param>
        /// <param name="timestamp">timestamp which harmony uses to order requests</param>
        /// <returns>Document for the command to send to the harmony hub.</returns>
        public static Document IrCommandDocument(string deviceId, string command, bool press = true, int? timestamp = null)
        {
            // Create a json representation of a harmony action request
            var json = Serializer.ToJson(new HarmonyAction
            {
                Type = "IRCommand",
                DeviceId = deviceId,
                Command = command
            });

            // At this point our valid json won't work - we need to break it so it looks like:
            // {"type"::"IRCommand","deviceId"::"deviceId","command"::"command"}
            // note double colons 

            json = json.Replace(":", "::");

            // Build the action to pass to harmony 
            var actionBuilder = new StringBuilder();
            // TODO: "hold" is also an accepted status, what else and how can we work with them?
            // See here for more information: https://github.com/jterrace/pyharmony/blob/master/PROTOCOL.md
            actionBuilder.Append("status=").Append(press ? "press" : "release").Append(':');
            actionBuilder.Append("action=").Append(json);
            if (timestamp.HasValue)
            {
                actionBuilder.Append(':').Append("timestamp=").Append(timestamp.Value);
            }

            // Now create and return the document
            return CreateDocument(HarmonyCommands.holdAction, actionBuilder.ToString());
        }

        /// <summary>
        ///     Create a "pair" document, this is a bit different from the others
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

        /// <summary>
        ///     Create a document to start an activity
        /// </summary>
        /// <param name="activityId">Id for the activity</param>
        /// <returns>Document</returns>
        public static Document StartActivityDocument(string activityId)
        {
            return CreateDocument(HarmonyCommands.startactivity, $"activityId={activityId}:timestamp=0");
        }
    }
}