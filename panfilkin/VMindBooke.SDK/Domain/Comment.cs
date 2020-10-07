using System;
using System.Collections.Generic;

namespace VMindBooke.SDK.Domain
{
    public class Comment
    {
        public Guid Id { get; }
        public int AuthorId { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public IReadOnlyCollection<Comment> Replies { get; }
        public IReadOnlyCollection<Like> Likes { get; }

        public Comment(Guid id, int authorId, string content, DateTime postingDateUtc, IReadOnlyCollection<Comment> replies, IReadOnlyCollection<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Replies = replies;
            Likes = likes;
        }
    }
}