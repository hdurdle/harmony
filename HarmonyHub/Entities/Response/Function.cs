using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    /// TODO: Document this
    /// </summary>
    [DataContract]
    public class Function
    {
        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "action")]
        public string Action { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// ToString will return the name of the function with the label behind it.
        /// </summary>
        public override string ToString()
        {
            return $"{Name} ({Label})";
        }
    }
}