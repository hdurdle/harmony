using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    [DataContract]
    public class ControlGroup
    {
        [DataMember(Name = "function")]
        public IList<Function> Functions { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}