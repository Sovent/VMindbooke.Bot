using System.Collections.Generic;
using VMBook.SDK;

namespace VMBookeBot.Domain
{
    public interface IRequestFilter
    {
        public Post GetMostPopularUserPost(int userId);
        public User GetUser(int id);
    }
}