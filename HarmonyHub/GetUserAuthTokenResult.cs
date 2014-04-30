namespace HarmonyHub
{
    /// <summary>
    /// Result of call to myharmony.com web service. Contains UserAuthToken.
    /// AccountId is always (so far) 0.
    /// </summary>
    public class GetUserAuthTokenResult
    {
        public int AccountId { get; set; }
        public string UserAuthToken { get; set; }
    }

    public class GetUserAuthTokenResultRootObject
    {
        public GetUserAuthTokenResult GetUserAuthTokenResult { get; set; }
    }
}
