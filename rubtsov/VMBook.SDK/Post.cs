using System;
using System.Collections;
using System.Collections.Generic;

namespace VMBook.SDK
{
    public class Post : IEnumerable<Comment>
    {
        public Post()
        {
        }
        public Post(int id, int authorId, List<Comment> comments)
        {
            Id = id;
            AuthorId = authorId;
            Comments = comments;
        }
        public int Id { get; set; }
        
        public int AuthorId { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        public DateTime PostingDateUtc { get; set; }
        
        public List<Comment> Comments { get; set; }
        
        public List<Like> Likes { get; set; }
        public IEnumerator<Comment> GetEnumerator()
        {
            return Comments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}