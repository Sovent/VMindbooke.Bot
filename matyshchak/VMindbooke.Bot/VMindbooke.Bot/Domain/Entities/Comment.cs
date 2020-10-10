using System;
using System.Collections.Generic;
using System.Linq;

namespace Usage.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        
        public int AuthorId { get; set; }
        
        public string Content { get; set; }
        
        public DateTime PostingDateUtc { get; set; }
        
        public List<Comment> Replies { get; set; }
        
        public List<Like> Likes { get; set; }

        public IEnumerable<Comment> GetAllReplies()
        {
            return Replies.Concat(Replies.SelectMany(r => r.GetAllReplies()));
        }
        
        public void Reply(int replyAuthorId, string replyMessage)
        {
            lock (Replies)
            {
                Replies.Add(new Comment()
                {
                    Id = Guid.NewGuid(),
                    Content = replyMessage,
                    AuthorId = replyAuthorId,
                    PostingDateUtc = DateTime.UtcNow,
                    Likes = new List<Like>(),
                    Replies = new List<Comment>()
                });
            }
        }
    }
}