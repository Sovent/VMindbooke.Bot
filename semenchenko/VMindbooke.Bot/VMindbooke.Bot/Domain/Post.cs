using System;
using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class Post
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PostingDateUtc { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Like> Likes { get; set; }
    }
}