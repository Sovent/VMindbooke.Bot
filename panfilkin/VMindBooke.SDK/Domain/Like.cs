using System;

namespace VMindBooke.SDK.Domain
{
    public class Like
    {
        public Guid Id { get; }
        public int AuthorId { get; }
        public DateTime PlacingDateUtc { get; }

        public Like(Guid id, int authorId, DateTime placingDateUtc)
        {
            Id = id;
            AuthorId = authorId;
            PlacingDateUtc = placingDateUtc;
        }
    }
}