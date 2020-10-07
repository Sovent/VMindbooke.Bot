using System;

namespace VMindbooke.Bot.Domain
{
    public class Like
    {
        public string Id { get; }
        
        public int AuthorId { get; }
        
        public DateTime PlacingDate { get; }
    }
}