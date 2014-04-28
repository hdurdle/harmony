namespace Harmony
{
    public class GetUserAuthTokenResult
    {
        public int AccountId { get; set; }
        public string UserAuthToken { get; set; }
    }

    public class RootObject
    {
        public GetUserAuthTokenResult GetUserAuthTokenResult { get; set; }
    }
}
