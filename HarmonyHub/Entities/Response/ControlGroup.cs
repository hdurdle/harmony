using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    /// <summary>
    /// TODO: Document this
    /// </summary>
    [DataContract]
    public class ControlGroup
    {
        /// <summary>
        /// TODO: Document this
        /// </summary>
        [DataMember(Name = "function")]
        public IList<Function> Functions { get; set; }

        /// <summary>
        /// Name of the control group
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Returns the name of the control group as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}