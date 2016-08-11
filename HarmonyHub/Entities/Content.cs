using System.Runtime.Serialization;


namespace HarmonyHub.Entities
{
    [DataContract]
    public class Content
    {
        [DataMember(Name = "contentUserHost")]
        public string UserHost { get; set; }
        [DataMember(Name = "contentDeviceHost")]
        public string DeviceHost { get; set; }
        [DataMember(Name = "contentServiceHost")]
        public string ServiceHost { get; set; }
        [DataMember(Name = "contentImageHost")]
        public string ImageHost { get; set; }
        [DataMember(Name = "householdUserProfileUri")]
        public string HouseholdUserProfileUri { get; set; }
    }
}