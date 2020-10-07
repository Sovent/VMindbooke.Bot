using System;
using System.Collections.Generic;
using System.Text.Json;

namespace VMindBooke.SDK.Domain
{
    public class Post
    {
        public int Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        
        public IReadOnlyCollection<Comment> Comments { get; }
        public IReadOnlyCollection<Like> Likes { get; }

        public Post(int id, int authorId, string title, string content, DateTime postingDateUtc, IReadOnlyCollection<Comment> comments, IReadOnlyCollection<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Comments = comments;
            Likes = likes;
        }
    }
}