using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class User
    {
        public User(string name, List<Like> likes)
        {
            Name = name;
            Likes = likes;
        }

        public string Name { get; }

        public List<Like> Likes { get; }
    }
}