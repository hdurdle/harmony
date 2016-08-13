using System.Runtime.Serialization;

namespace HarmonyHub.Entities
{
    [DataContract]
    public class Function
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "action")]
        public string Action { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Label})";
        }
    }
}
