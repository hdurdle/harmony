
namespace HarmonyHub
{
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
