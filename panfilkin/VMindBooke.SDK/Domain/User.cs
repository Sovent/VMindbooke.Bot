using System.Collections.Generic;
using System.Dynamic;

namespace VMindBooke.SDK.Domain
{
    public class User
    {
        public int Id { get; }
        public string Token { get; }
        public string Name { get; }
        public IReadOnlyCollection<Like> LikesList { get; }

        public User(int id, string token, string name, IReadOnlyCollection<Like> likesList)
        {
            Id = id;
            Token = token;
            Name = name;
            LikesList = likesList ?? new List<Like>();
        }
    }
}