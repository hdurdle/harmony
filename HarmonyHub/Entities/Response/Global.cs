using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    /// TODO: Document this
    /// </summary>
    [DataContract]
    public class Global
    {
        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "timeStampHash")]
        public string TimeStampHash { get; set; }
    }
}