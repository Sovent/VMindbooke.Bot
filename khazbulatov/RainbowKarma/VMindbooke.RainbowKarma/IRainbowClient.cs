using System.Collections.Generic;
using VMindbooke.SDK.Model;

namespace VMindbooke.RainbowKarma
{
    public interface IRainbowClient
    {
        IEnumerable<Post> GetPosts();
        IEnumerable<User> GetUsers();
        IEnumerable<Post> GetUserPosts(User user);
        void Register();
        void Comment(Post post);
        void Reply(Post post, Comment comment);
        void Repost(Post post);
    }
}