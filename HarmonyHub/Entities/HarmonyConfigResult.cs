using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HarmonyHub.Entities
{
    /// <summary>
    /// Logitech HarmonyHub Configuration
    /// </summary>
    [DataContract]
    public class HarmonyConfigResult
    {
        [DataMember(Name = "activity")]
        public IList<Activity> Activity { get; set; }
        [DataMember(Name = "device")]
        public IList<Device> Device { get; set; }
        [DataMember(Name = "sequence")]
        public IList<object> Sequence { get; set; }
        [DataMember(Name = "content")]
        public Content Content { get; set; }
        [DataMember(Name = "global")]
        public Global Global { get; set; }

        public string ActivityNameFromId(string activityId)
        {
            return (from activityItem in Activity where activityItem.Id == activityId select activityItem.Label).FirstOrDefault();
        }

        public string DeviceNameFromId(string deviceId)
        {
            return (from deviceItem in Device where deviceItem.Id == deviceId select deviceItem.Label).FirstOrDefault();
        }
    }
}