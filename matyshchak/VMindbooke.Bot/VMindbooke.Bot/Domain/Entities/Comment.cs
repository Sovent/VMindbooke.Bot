using System;
using System.Collections.Generic;

namespace Usage.Domain.Entities
{
    public class Comment
    {
        public Comment(Guid id,
            int authorId,
            string content,
            DateTime postingDateUtc,
            List<Comment> replies,
            List<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Replies = replies;
            Likes = likes;
        }
        
        public Guid Id { get; }
        public int AuthorId { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public List<Comment> Replies { get; }
        public List<Like> Likes { get; }
    }
}