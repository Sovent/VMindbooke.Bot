using System;

namespace VMindbooke.Bot.Domain
{
    public class Like
    {
        public Like(string id, int authorId, DateTime placingDateUtc)
        {
            Id = id;
            AuthorId = authorId;
            PlacingDateUtc = placingDateUtc;
        }
        
        public string Id { get; }
        
        public int AuthorId { get; }
        
        public DateTime PlacingDateUtc { get; }
    }
}