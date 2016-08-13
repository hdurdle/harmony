using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Auth
{
    [DataContract]
    public class Credentials
    {
        [DataMember(Name = "email")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

}
