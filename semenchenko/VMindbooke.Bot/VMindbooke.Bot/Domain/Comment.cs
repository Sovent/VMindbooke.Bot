using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class Comment
    {
        public string Id { get; }

        public int AuthorId { get; }

        public string Content { get; }

        public List<Like> Likes { get; }

        public List<Comment> Replies { get; }
    }
}