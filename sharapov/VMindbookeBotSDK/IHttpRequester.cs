using VMindbookeBot;

namespace VMindbookeBotSDK
{
    public interface IHttpRequester
    {
        Result<Post, HttpRequester.StatusCode> GetPost(int postId);
        HttpRequester.StatusCode WriteComment(int postId, string contentText);
        HttpRequester.StatusCode WriteReply(int postId, string commentId, string contentText);
        Result<int, HttpRequester.StatusCode> WriteNewPost(int userId, string titleText, string contentText);
        Result<Post[], HttpRequester.StatusCode> GetUserPosts(int userId);
        Result<UserInfoByUserId, HttpRequester.StatusCode> GetUserInfo(int userId);
    }
}