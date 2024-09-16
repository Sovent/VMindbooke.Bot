using System;

namespace VMindbooke.Bot.Domain
{
    public class CommentToReply
    {
        public CommentToReply(string id, Like[] likes)
        {
            Id = id;
            Likes = likes;
        }
        
        public string Id { get; }
        
        public Like[] Likes { get; }
    }
}