using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public interface IPostsRepository
    {
        IReadOnlyCollection<Post> GetPosts();
    }
}