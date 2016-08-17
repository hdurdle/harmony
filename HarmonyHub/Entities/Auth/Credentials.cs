using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Auth
{
    /// <summary>
    /// Credentials for the Authentication process between the client and the MyHarmony site
    /// </summary>
    [DataContract]
    public class Credentials
    {
        /// <summary>
        /// Username at MyHarmony, usualls the email adress
        /// </summary>
        [DataMember(Name = "email")]
        public string Username { get; set; }

        /// <summary>
        /// The password at MyHarmony
        /// </summary>
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

}
