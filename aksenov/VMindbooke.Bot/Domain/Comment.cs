using System;
using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class Comment
    {
        public string Id { get; }
        
        public int AuthorId { get; }
        
        public string Content { get; }
        
        public DateTime PlacingDate { get; }
        
        public IEnumerable<Comment> Replies { get; }

        public IEnumerable<Like> Likes { get; }
    }
}