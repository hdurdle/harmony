using System.Runtime.Serialization;

namespace HarmonyHub.Entities
{
    /// <summary>
    /// HarmonyHub Remote Action
    /// </summary>
    [DataContract]
    public class HarmonyAction
    {
        /// <summary>
        /// Action Type (IRCommand)
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// DeviceId to receive command
        /// </summary>
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }

        /// <summary>
        /// HarmonyHub command to send to device
        /// </summary>
        [DataMember(Name = "command")]
        public string Command { get; set; }
    }
}