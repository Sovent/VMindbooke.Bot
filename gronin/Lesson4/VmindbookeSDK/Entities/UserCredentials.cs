using System;

namespace VMindbookeSDK.Entities
{
    public class UserCredentials
    {
        public int Id { get; }
        public string Token { get; }

        public UserCredentials(int id, string token)
        {
            Id = id;
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}
