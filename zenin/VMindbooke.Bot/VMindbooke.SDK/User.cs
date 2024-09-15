using System.Collections.Generic;

namespace VMindbooke.SDK
{
    public class User
    {
        public User(int id, string name, IReadOnlyCollection<Like> likes)
        {
            Id = id;
            Name = name;
            Likes = likes;
        }
        public int Id { get; }
        public string Name { get; }
        public IReadOnlyCollection<Like> Likes { get;}
    }
}