using System;

namespace VMindbooke.SDK.Model
{
    public class UserCredentials
    {
        public UserCredentials(int id, string token)
        {
            Id = id;
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public int Id { get; }
        public string Token { get; }
    }
}
