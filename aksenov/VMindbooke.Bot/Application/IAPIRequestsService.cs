using System.Collections.Generic;
using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Application
{
    public interface IAPIRequestsService
    {
        IEnumerable<Post> GetPosts();
        IEnumerable<User> GetUsers();
        IEnumerable<Post> GetUserPosts(int userId);
        User GetUser(int userId);
        bool PostComment(int postId, string content, string userToken);
        bool ReplyToComment(int postId, string commentId, string content, string userToken);
        bool CreatePost(int userId, string userToken, string title, string content);
        User CreateUser(string name);
    }
}