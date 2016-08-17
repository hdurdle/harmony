using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    /// TODO: Document this
    /// </summary>
    [DataContract]
    public class Content
    {
        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "contentDeviceHost")]
        public string DeviceHost { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "householdUserProfileUri")]
        public string HouseholdUserProfileUri { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "contentImageHost")]
        public string ImageHost { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "contentServiceHost")]
        public string ServiceHost { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "contentUserHost")]
        public string UserHost { get; set; }
    }
}