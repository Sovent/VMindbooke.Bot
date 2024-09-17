using System.Collections.Generic;
using VMindBooke.SDK.Domain;

namespace VMindBooke.SDK.Application
{
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers();
        User GetUser(int id);
        IEnumerable<Post> GetUserPosts(User user);
        User CreateUser(string username);
        void CreatePost(User actor, string title, string content);
        User GetAuthorizedUser(int id, string token);
        User UserLikesUpdate(User user);
    }
}