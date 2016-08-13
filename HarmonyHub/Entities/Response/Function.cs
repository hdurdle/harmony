using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Response
{
    [DataContract]
    public class Function
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Label})";
        }
    }
}