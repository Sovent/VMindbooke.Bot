using System;

namespace VMindbooke.SDK
{
    public class Like
    {
        public Like(string id, int authorId, DateTime date)
        {
            Id = id;
            AuthorId = authorId;
            PlacingDateUtc = date;
        }
        public string Id { get; }
        public int AuthorId { get; }
        public DateTime PlacingDateUtc { get; }
        
    }
}