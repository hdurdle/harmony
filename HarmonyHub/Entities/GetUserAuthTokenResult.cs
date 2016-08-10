using System.Runtime.Serialization;

namespace HarmonyHub.Entities
{
    /// <summary>
    /// Result of call to myharmony.com web service. Contains UserAuthToken.
    /// AccountId is always (so far) 0.
    /// </summary>
    [DataContract]
    public class GetUserAuthTokenResult
    {
        [DataMember(Name = "AccountId")]
        public int AccountId { get; set; }
        [DataMember(Name = "UserAuthToken")]
        public string UserAuthToken { get; set; }
    }
}