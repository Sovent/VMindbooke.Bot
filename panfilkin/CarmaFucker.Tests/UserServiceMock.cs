using System.Collections.Generic;
using VMindBooke.SDK.Application;
using VMindBooke.SDK.Domain;

namespace CarmaFucker.Tests
{
    public class UserServiceMock : IUserService
    {
        public IEnumerable<User> GetAllUsers()
        {
            var userList = new List<User> {GetUser(11), GetUser(22), GetUser(33)};
            return userList;
        }

        public User GetUser(int id)
        {
            return FakeDataGenerator.GetUser(id);
        }

        public IEnumerable<Post> GetUserPosts(User user)
        {
            var postList = new List<Post>
            {
                FakeDataGenerator.GetPost(-1, 3),
                FakeDataGenerator.GetPost(-1, 3),
                FakeDataGenerator.GetPost(-1, 3),
                FakeDataGenerator.GetPost(-1, 3),
                FakeDataGenerator.GetPost(-1, 3)
            };
            return postList;
        }

        public User CreateUser(string username)
        {
            return FakeDataGenerator.GetUser(55);
        }

        public void CreatePost(User actor, string title, string content)
        {
        }

        public User GetAuthorizedUser(int id, string token)
        {
            return GetUser(id);
        }

        public User UserLikesUpdate(User user)
        {
            return user;
        }
    }
}