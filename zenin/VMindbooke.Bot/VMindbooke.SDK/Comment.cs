using System;
using System.Collections.Generic;

namespace VMindbooke.SDK
{
    public class Comment
    {
        public Comment(string id, int authorId, string content, DateTime date,
            IReadOnlyCollection<Reply> replies, IReadOnlyCollection<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PlacingDateUtc = date;
            Replies = replies;
            Likes = likes;
        }
        public string Id { get; }
        public int AuthorId { get; }
        public string Content { get; }
        public DateTime PlacingDateUtc { get; }
        public IReadOnlyCollection<Reply> Replies { get;}
        public IReadOnlyCollection<Like> Likes { get;}
    }
}