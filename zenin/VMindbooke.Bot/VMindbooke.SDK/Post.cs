using System;
using System.Collections.Generic;

namespace VMindbooke.SDK
{
    public class Post
    {
        public Post(int id, int authorId, string title, string content, DateTime date, 
            IReadOnlyCollection<Comment> comments, IReadOnlyCollection<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Content = content;
            PostingDateUtc = date;
            Comments = comments;
            Likes = likes;
        }
        public int Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public IReadOnlyCollection<Comment> Comments { get; }
        public IReadOnlyCollection<Like> Likes { get; }
    }
}