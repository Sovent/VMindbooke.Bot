using System;
using System.Collections.Generic;

namespace VMindBooke.SDK.Domain
{
    public class Post
    {
        public int Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }

        public IReadOnlyList<Comment> Comments { get; }
        public IReadOnlyList<Like> Likes { get; }

        public Post(int id, int authorId, string title, string content, DateTime postingDateUtc,
            IReadOnlyList<Comment> comments, IReadOnlyList<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Comments = comments ?? new List<Comment>();
            Likes = likes ?? new List<Like>();
        }
    }
}