using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace HarmonyHub.Entities
{
    [DataContract]
    public class ControlGroup
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "function")]
        public IList<Function> Functions { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
