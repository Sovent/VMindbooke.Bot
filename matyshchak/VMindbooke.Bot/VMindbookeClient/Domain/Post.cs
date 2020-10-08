using System;
using System.Collections.Generic;
using System.Linq;

namespace VMindbookeClient.Domain
{
    public class Post
    {
        public int Id { get; set; }
        
        public int AuthorId { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        public DateTime PostingDateUtc { get; set; }
        
        public List<Comment> Comments { get; set; }
        
        public List<Like> Likes { get; set; }

        public Comment TryFindCommentById(Guid commentId)
        {
            return GetAllComments().FirstOrDefault(comment => comment.Id == commentId);
        }

        public IEnumerable<Comment> GetAllComments()
        {
            return Comments.Concat(Comments.SelectMany(c => c.GetAllReplies()));
        }

        public Guid Comment(int authorId, string content)
        {
            var id = Guid.NewGuid();
            lock (Comments)
            {
                Comments.Add(new Comment()
                {
                    AuthorId = authorId,
                    Content = content,
                    Id = id,
                    Likes = new List<Like>(),
                    PostingDateUtc = DateTime.UtcNow,
                    Replies = new List<Comment>()
                });
            }
            
            return id;
        }
    }
}