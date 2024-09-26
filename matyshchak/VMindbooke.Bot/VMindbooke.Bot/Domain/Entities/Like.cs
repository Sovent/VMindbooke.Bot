using System;

namespace Usage.Domain.Entities
{
    public class Like
    {
        public Like(Guid id, int authorId, DateTime placingDateUtc)
        {
            Id = id;
            AuthorId = authorId;
            PlacingDateUtc = placingDateUtc;
        }
        
        public Guid Id { get; }
        public int AuthorId { get; }
        public DateTime PlacingDateUtc { get; }
    }
}