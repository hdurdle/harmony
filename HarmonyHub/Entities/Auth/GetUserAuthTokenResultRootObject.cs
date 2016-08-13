using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Auth
{
    [DataContract]
    public class GetUserAuthTokenResultRootObject
    {
        [DataMember(Name = "GetUserAuthTokenResult")]
        public GetUserAuthTokenResult GetUserAuthTokenResult { get; set; }
    }
}