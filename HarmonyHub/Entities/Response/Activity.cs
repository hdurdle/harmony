using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    ///     HarmonyHub Activity
    /// </summary>
    [DataContract]
    public class Activity : IComparable<Activity>
    {
        /// <summary>
        /// Sort-rder for the activity
        /// </summary>
        [DataMember(Name = "activityOrder")]
        public int ActivityOrder { get; set; }

        /// <summary>
        /// Display name for the activity type
        /// </summary>
        [DataMember(Name = "activityTypeDisplayName")]
        public string ActivityTypeDisplayName { get; set; }

        /// <summary>
        /// Url for the image
        /// </summary>
        [DataMember(Name = "baseImageUri")]
        public string BaseImageUri { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "controlGroup")]
        public IList<ControlGroup> ControlGroups { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "fixit")]
        public Dictionary<string, FixItCommand> Fixit { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Id for the activity
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "isTuningDefault")]
        public bool IsTuningDefault { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// TODO: Needs documentation, and a type
        /// </summary>
        [DataMember(Name = "sequences")]
        public IList<object> Sequences { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "suggestedDisplay")]
        public string SuggestedDisplay { get; set; }

        /// <summary>
        /// TODO: Needs documentation
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Used for sorting the times
        /// </summary>
        public int CompareTo(Activity other)
        {
            return string.Compare(Label, other?.Label, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}