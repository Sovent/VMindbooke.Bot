using System;
using System.Collections.Generic;

namespace VMindbooke.SDK.Model
{
    public class Comment
    {
        public int Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public IEnumerable<Like> Likes { get; }
        public IEnumerable<Comment> Replies { get; }

        public Comment(int id, int authorId, string title, string content,
            DateTime postingDateUtc, IEnumerable<Like> likes, IEnumerable<Comment> comments)
        {
            Id = id;
            AuthorId = authorId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            PostingDateUtc = postingDateUtc;
            Likes = likes ?? throw new ArgumentNullException(nameof(likes));
            Replies = comments ?? throw new ArgumentNullException(nameof(comments));
        }
    }
}
