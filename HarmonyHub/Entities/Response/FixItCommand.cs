using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    ///     Power and Input states to "fix" a device
    /// </summary>
    [DataContract]
    public class FixItCommand
    {
        /// <summary>
        ///     Device ID for fix settings
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "Input")]
        public string Input { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "isManualPower")]
        public bool IsManualPower { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "isRelativePower")]
        public bool IsRelativePower { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "Power")]
        public string Power { get; set; }
    }
}