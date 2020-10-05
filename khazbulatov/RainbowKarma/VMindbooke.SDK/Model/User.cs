using System;
using System.Collections.Generic;

namespace VMindbooke.SDK.Model
{
    public class User
    {
        public int Id { get; }
        public string Name { get; }
        public IEnumerable<Like> Likes { get; }

        public User(int id, string name, IEnumerable<Like> likes)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Likes = likes ?? throw new ArgumentNullException(nameof(likes));
        }
    }
}
