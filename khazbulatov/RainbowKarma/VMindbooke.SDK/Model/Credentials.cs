using System;

namespace VMindbooke.SDK.Model
{
    public class Credentials
    {
        public string Token { get; }

        public Credentials(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}
