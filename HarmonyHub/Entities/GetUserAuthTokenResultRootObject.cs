using System.Runtime.Serialization;


namespace HarmonyHub.Entities
{
    [DataContract]
    public class GetUserAuthTokenResultRootObject
    {
        [DataMember(Name = "GetUserAuthTokenResult")]
        public GetUserAuthTokenResult GetUserAuthTokenResult { get; set; }
    }
}