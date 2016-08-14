using System.Runtime.Serialization;

namespace HarmonyHub.Entities.Auth
{
    /// <summary>
    ///     Result of call to myharmony.com web service.
    ///     AccountId is always (so far) 0.
    /// </summary>
    [DataContract]
    public class GetUserAuthTokenResult
    {
        /// <summary>
        /// ID of the account
        /// </summary>
        [DataMember(Name = "AccountId")]
        public int AccountId { get; set; }

        /// <summary>
        /// Auth-Token for accessing the HarmonyHub, this can be used to get an acces token.
        /// </summary>
        [DataMember(Name = "UserAuthToken")]
        public string UserAuthToken { get; set; }
    }
}