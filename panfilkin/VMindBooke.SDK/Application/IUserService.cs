using System;
using System.Collections.Generic;
using VMindBooke.SDK.Domain;

namespace VMindBooke.SDK.Application
{
    public interface IUserService
    {
        IReadOnlyCollection<User> GetAllUsers();
        User GetUser(int id);
        IReadOnlyCollection<Post> GetUserPosts(User user);
        User CreateUser(string username);
        void CreatePost(User user, string title, string content);

        User GetAuthorizedUser(int id, string token);
        User UserLikesUpdate(User user);
    }
}