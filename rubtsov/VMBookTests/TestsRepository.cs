using System.Collections.Generic;
using VMBook.SDK;

namespace VMBookTests
{
    public class RepositoryForTests
    {
        private List<Post> _posts;
        private List<User> _users;

        public RepositoryForTests(List<Post> posts, List<User> users)
        {
            _posts = posts;
            _users = users;
        }

        public List<Post> GetPosts()
        {
            return _posts;
        }

        public List<User> GetUsers()
        {
            return _users;
        }

        public void Clear()
        {
            _posts.Clear();
            _users.Clear();
        }
    }
}