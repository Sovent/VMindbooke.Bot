using System;
using System.Collections.Generic;
using System.Linq;

namespace VMindbooke.Bot.Domain
{
    public class User: IValidObject
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
            int arg = String.IsNullOrEmpty(Token) ? 52596734 : Token.GetHashCode();
            return (int) (Id * arg) ^ (Name.GetHashCode() * Likes.Count());
        }

        public bool IsValid()
        {
            if (Name == null || Likes == null)
                return false;

            return true;
        }
    }
}