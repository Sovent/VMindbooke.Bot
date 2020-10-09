using System;
using System.Collections.Generic;

namespace VMindbooke.SDK.Model
{
    public class Comment
    {
        public Guid Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public IReadOnlyList<Like> Likes { get; }
        public IReadOnlyList<Comment> Replies { get; }

        public Comment(Guid id, int authorId, string title, string content,
            DateTime postingDateUtc, IReadOnlyList<Like> likes, IReadOnlyList<Comment> comments)
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
