using System.Collections.Generic;

namespace VMindBooke.SDK.Domain
{
    public class User
    {
        public int Id { get; }
        public string Token { get; }
        public string Name { get; }
        public IReadOnlyList<Like> Likes { get; }

        public User(int id, string token, string name, IReadOnlyList<Like> likes)
        {
            Id = id;
            Token = token ?? "";
            Name = name;
            Likes = likes ?? new List<Like>();
        }
    }
}