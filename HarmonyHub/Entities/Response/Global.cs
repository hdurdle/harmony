using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    [DataContract]
    public class Global
    {
        [DataMember(Name = "locale")]
        public string Locale { get; set; }

        [DataMember(Name = "timeStampHash")]
        public string TimeStampHash { get; set; }
    }
}