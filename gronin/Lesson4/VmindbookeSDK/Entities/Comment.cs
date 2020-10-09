using System;
using System.Collections.Generic;
using VMindbookeSDK.Entities;

namespace VmindbookeSDK.Entities
{
    public class Comment
    {
        public Guid Id { get; }
        public int AuthorId { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public IEnumerable<Comment> Replies { get; }
        public IEnumerable<Like> Likes { get; }

        public Comment(Guid id, int authorId, string title, string content,
            DateTime postingDateUtc, IEnumerable<Like> likes, IEnumerable<Comment> comments)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Likes = likes;
            Replies = comments;
        }
    }
}
