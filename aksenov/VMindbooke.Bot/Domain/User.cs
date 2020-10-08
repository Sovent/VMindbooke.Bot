using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VMindbooke.Bot.Domain
{
    public class User
    {
        public User(int id, string token, string name, IEnumerable<Like> likes)
        {
            Id = id;
            Token = token;
            Name = name;
            Likes = likes;
        }
        
        public int Id { get; }
        
        public string Token { get; }
        
        public string Name { get; }
        
        public IEnumerable<Like> Likes { get; }

        public override int GetHashCode()
        {
            return (Id * Token.GetHashCode()) ^ (Name.GetHashCode() * Likes.Count());
        }
    }
}