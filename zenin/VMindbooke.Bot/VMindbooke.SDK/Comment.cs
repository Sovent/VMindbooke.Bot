using System;
using System.Collections.Generic;

namespace VMindbooke.SDK
{
    public class Comment
    {
        public Comment(Guid id, int authorId, string content, DateTime placingDateUtc,
            IReadOnlyCollection<Reply> replies, IReadOnlyCollection<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PlacingDateUtc = placingDateUtc;
            Replies = replies;
            Likes = likes;
        }
        public Guid Id { get; }
        public int AuthorId { get; }
        public string Content { get; }
        public DateTime PlacingDateUtc { get; }
        public IReadOnlyCollection<Reply> Replies { get;}
        public IReadOnlyCollection<Like> Likes { get;}
    }
}