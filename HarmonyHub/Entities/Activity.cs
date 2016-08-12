using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace HarmonyHub.Entities
{
    /// <summary>
    /// HarmonyHub Activity
    /// </summary>    
    [DataContract]
    public class Activity : IComparable<Activity>
    {
        [DataMember(Name = "suggestedDisplay")]
        public string SuggestedDisplay { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "activityTypeDisplayName")]
        public string ActivityTypeDisplayName { get; set; }
        [DataMember(Name = "controlGroup")]
        public IList<ControlGroup> ControlGroups { get; set; }
        [DataMember(Name = "sequences")]
        public IList<object> Sequences { get; set; }
        [DataMember(Name = "activityOrder")]
        public int ActivityOrder { get; set; }
        [DataMember(Name = "isTuningDefault")]
        public bool IsTuningDefault { get; set; }
        [DataMember(Name = "fixit")]
        public Dictionary<string, FixItCommand> Fixit { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "icon")]
        public string Icon { get; set; }
        [DataMember(Name = "baseImageUri")]
        public string BaseImageUri { get; set; }

        public int CompareTo(Activity other)
        {
            return string.Compare(Label, other?.Label, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}