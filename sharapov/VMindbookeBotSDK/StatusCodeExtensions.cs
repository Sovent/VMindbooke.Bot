using VMindbookeBotSDK;

namespace VMindbookeBot
{
    public static class StatusCodeExtensions
    {
        public static bool IsPostError(this HttpRequester.StatusCode error)
        {
            return error == HttpRequester.StatusCode.InternalServerError;
        }
        
        public static bool IsGetError(this HttpRequester.StatusCode error)
        {
            return error == HttpRequester.StatusCode.NotFound;
        }
        
        public static bool IsError(this HttpRequester.StatusCode error)
        {
            return error.IsPostError() || error.IsGetError();
        }
    }
}