using System;
using System.Collections.Generic;
using VmindbookeSDK.Entities;

namespace VMindbookeSDK.Entities
{
    public class Post
    {
        public int Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public IEnumerable<Like> Likes { get; }
        public IEnumerable<Comment> Comments { get; }

        public Post(int id, int authorId, string title, string content, DateTime postingDateUtc,
            IEnumerable<Like> likes, IEnumerable<Comment> comments)
        {
            Id = id;
            AuthorId = authorId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            PostingDateUtc = postingDateUtc;
            Likes = likes ?? throw new ArgumentNullException(nameof(likes));
            Comments = comments ?? throw new ArgumentNullException(nameof(comments));
        }
    }
}
