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
        public IReadOnlyList<Comment> Replies { get; }
        public IReadOnlyList<Like> Likes { get; }

        public Comment(Guid id, int authorId, string content, DateTime postingDateUtc, IReadOnlyList<Comment> replies,
            IReadOnlyList<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Replies = replies ?? new List<Comment>();
            Likes = likes ?? new List<Like>();
        }
    }
}