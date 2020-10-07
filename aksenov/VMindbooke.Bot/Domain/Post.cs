using System;
using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class Post
    {
        public int Id { get; }

        public int AuthorId { get; }

        public string Title { get; }

        public string Content { get; }

        public DateTime PostingDate { get; }

        public IEnumerable<Comment> Comments { get; }

        public IEnumerable<Like> Likes { get; }
    }
}