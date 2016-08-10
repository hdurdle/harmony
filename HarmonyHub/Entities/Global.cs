using System.Runtime.Serialization;

namespace HarmonyHub.Entities
{
    [DataContract]
    public class Global
    {
        [DataMember(Name = "timeStampHash")]
        public string TimeStampHash { get; set; }
        [DataMember(Name = "locale")]
        public string Locale { get; set; }
    }
}