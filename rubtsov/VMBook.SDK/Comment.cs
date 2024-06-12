using System;
using System.Collections.Generic;

namespace VMBook.SDK
{
    public class Comment
    {
        public Guid Id { get; set; }
        
        public int AuthorId { get; set; }
        
        public string Content { get; set; }
        
        public DateTime PostingDateUtc { get; set; }
        
        public List<Comment> Replies { get; set; }
        
        public List<Like> Likes { get; set; }
    }
}