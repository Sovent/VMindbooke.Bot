using System.Collections.Generic;

namespace VMBook.SDK
{
    public interface IVmbClientFilter
    {
        public User GetUser(int id);
        public IReadOnlyCollection<User> GetUsers();
        public IReadOnlyCollection<Post> GetPosts();
        public IReadOnlyCollection<Post> GetUserPosts(int userId);
    }
}