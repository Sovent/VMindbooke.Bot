using System;
using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class Comment: IValidObject
    {
        public Comment(string id, int authorId, string content, DateTime postingDateUtc, Comment[] replies, Like[] likes)
        {
            Id = id;
            AuthorId = authorId;
            Content = content;
            PostingDateUtc = postingDateUtc;
            Replies = replies;
            Likes = likes;
        }
        
        public string Id { get; }
        
        public int AuthorId { get; }
        
        public string Content { get; }
        
        public DateTime PostingDateUtc { get; }
        
        public Comment[] Replies { get; }

        public Like[] Likes { get; }
        
        public bool IsValid()
        {
            if (Id == null || Content == null || Replies == null || Likes == null)
                return false;

            return true;
        }
    }
}