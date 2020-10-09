using System.Collections.Generic;
using System.Linq;
using VMBookeBot.Domain;
using VMBook.SDK;

namespace VMBookTests
{
    public class TestsRequestFilter : IRequestFilter
    {
        private List<Post> _posts;
        private List<User> _users;

        public TestsRequestFilter(RepositoryForTests repository)
        {
            _posts = repository.GetPosts();
            _users = repository.GetUsers();
        }
        public Post GetMostPopularUserPost(int userId)
        {
            return _posts.Where(post => post.AuthorId == userId)
                .OrderByDescending(post => post.Likes.Count)
                .First();
        }

        public User GetUser(int id)
        {
            return _users.First(user => user.Id == id);
        }
    }
}