using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    ///     Logitech HarmonyHub Configuration
    /// </summary>
    [DataContract]
    public class Config
    {
        /// <summary>
        /// All the activities in the configuration
        /// </summary>
        [DataMember(Name = "activity")]
        public IList<Activity> Activities { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "content")]
        public Content Content { get; set; }

        /// <summary>
        /// Devices in the configuration
        /// </summary>
        [DataMember(Name = "device")]
        public IList<Device> Devices { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "global")]
        public Global Global { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "sequence")]
        public IList<object> Sequence { get; set; }

        /// <summary>
        /// Get the activity name for the activity ID
        /// TODO: This would actually be better fitted in an extension
        /// 
        /// </summary>
        public string ActivityNameFromId(string activityId)
        {
            return (from activityItem in Activities where activityItem.Id == activityId select activityItem.Label).FirstOrDefault();
        }

        /// <summary>
        /// Get the device name for the device ID
        /// TODO: This would actually be better fitted in an extension
        /// </summary>
        public string DeviceNameFromId(string deviceId)
        {
            return (from deviceItem in Devices where deviceItem.Id == deviceId select deviceItem.Label).FirstOrDefault();
        }
    }
}