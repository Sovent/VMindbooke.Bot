using System;
using System.Collections.Generic;

namespace VMindbooke.SDK.Model
{
    internal class CreatedUser
    {
        public int Id { get; }
        public string Name { get; }
        public string Token { get; }
        public IEnumerable<Like> Likes { get; }

        public CreatedUser(int id, string name, string token, IEnumerable<Like> likes)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Likes = likes ?? throw new ArgumentNullException(nameof(likes));
        }

        public (User, Credentials) Decompose() =>
            (new User(Id, Name, Likes), new Credentials(Token));
    }
}
