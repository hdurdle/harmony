using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HarmonyHub
{
    /// <summary>
    /// Logitech HarmonyHub Configuration
    /// </summary>
    public class HarmonyConfigResult
    {
        public List<Activity> activity { get; set; }
        public List<Device> device { get; set; }
        public List<object> sequence { get; set; }
        public Content content { get; set; }
        public Global global { get; set; }

        public string ActivityNameFromId(string activityId)
        {
            return (from activityItem in activity where activityItem.id == activityId select activityItem.label).FirstOrDefault();
        }

        public string DeviceNameFromId(string deviceId)
        {
            return (from deviceItem in device where deviceItem.id == deviceId select deviceItem.label).FirstOrDefault();
        }
    }

    /// <summary>
    /// HarmonyHub Activity
    /// </summary>
    public class Activity : IComparable<Activity>
    {
        public string suggestedDisplay { get; set; }
        public string label { get; set; }
        public string id { get; set; }
        public string activityTypeDisplayName { get; set; }
        public List<object> controlGroup { get; set; }
        public List<object> sequences { get; set; }
        public int activityOrder { get; set; }
        public bool isTuningDefault { get; set; }
        public Dictionary<string, FixItCommand> fixit { get; set; }
        public string type { get; set; }
        public string icon { get; set; }
        public string baseImageUri { get; set; }

        public int CompareTo(Activity other)
        {
            return String.Compare(label, other.label, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    /// <summary>
    /// HarmonyHub Device
    /// </summary>
    public class Device : IComparable<Device>
    {
        public int Transport { get; set; }
        public string suggestedDisplay { get; set; }
        public string deviceTypeDisplayName { get; set; }
        public string label { get; set; }
        public string id { get; set; }
        public List<int> Capabilities { get; set; }
        public string type { get; set; }
        public int DongleRFID { get; set; }
        public List<object> controlGroup { get; set; }
        public int ControlPort { get; set; }
        public bool IsKeyboardAssociated { get; set; }
        public string model { get; set; }
        public string deviceProfileUri { get; set; }
        public string manufacturer { get; set; }
        public string icon { get; set; }
        public string isManualPower { get; set; }

        public int CompareTo(Device other)
        {
            return String.Compare(label, other.label, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public class Content
    {
        public string contentUserHost { get; set; }
        public string contentDeviceHost { get; set; }
        public string contentServiceHost { get; set; }
        public string contentImageHost { get; set; }
        public string householdUserProfileUri { get; set; }
    }

    public class Global
    {
        public string timeStampHash { get; set; }
        public string locale { get; set; }
    }

    /// <summary>
    /// Power and Input states to "fix" a device
    /// </summary>
    [DataContract]
    public class FixItCommand
    {
        /// <summary>
        /// Device ID for fix settings
        /// </summary>
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public bool isRelativePower { get; set; }
        [DataMember]
        public bool isManualPower { get; set; }
        [DataMember]
        public string Power { get; set; }
        [DataMember]
        public string Input { get; set; }
    }

    /// <summary>
    /// HarmonyHub Remote Action
    /// </summary>
    public class HarmonyAction
    {
        /// <summary>
        /// Action Type (IRCommand)
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// DeviceId to receive command
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// HarmonyHub command to send to device
        /// </summary>
        public string command { get; set; }
    }
}
