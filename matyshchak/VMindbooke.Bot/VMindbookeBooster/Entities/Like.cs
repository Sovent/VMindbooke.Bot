using System;

namespace VMindbookeBooster.Entities
{
    public class Like
    {
        public Guid Id { get; set; }
        
        public int AuthorId { get; set; }
        
        public DateTime PlacingDateUtc { get; set; }
    }
}