using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Auth
{
    /// <summary>
    /// Root object for the authentication json object which is returned from the MyHarmony site
    /// </summary>
    [DataContract]
    public class GetUserAuthTokenResultRootObject
    {
        /// <summary>
        /// Contains the result
        /// </summary>
        [DataMember(Name = "GetUserAuthTokenResult")]
        public GetUserAuthTokenResult GetUserAuthTokenResult { get; set; }
    }
}