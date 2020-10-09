using System.Collections.Generic;

namespace VMBook.SDK
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Like> Likes { get; set; }
    }
}