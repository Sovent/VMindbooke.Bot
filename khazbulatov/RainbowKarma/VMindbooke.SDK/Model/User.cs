using System;
using System.Collections.Generic;

namespace VMindbooke.SDK.Model
{
    public class User
    {
        public User(int id, string name, IReadOnlyList<Like> likes)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Likes = likes ?? throw new ArgumentNullException(nameof(likes));
        }

        public int Id { get; }
        public string Name { get; }
        public IReadOnlyList<Like> Likes { get; }
    }
}
