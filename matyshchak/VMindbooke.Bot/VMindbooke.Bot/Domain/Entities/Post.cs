using System;
using System.Collections.Generic;
using System.Linq;

namespace Usage.Domain.Entities
{
    public class Post
    {
        public Post(int id,
            int authorId,
            string title,
            string content,
            DateTime postingDateUtc,
            List<Comment> comments,
            List<Like> likes)
        {
            Id = id;
            AuthorId = authorId;
            Title = title;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Comments = comments;
            Likes = likes;
        }
        
        public int Id { get; }
        public int AuthorId { get; }
        public string Title { get; }
        public string Content { get; }
        public DateTime PostingDateUtc { get; }
        public List<Comment> Comments { get; }
        public List<Like> Likes { get; }
    }
}