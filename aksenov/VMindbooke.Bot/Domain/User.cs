using System.Collections;
using System.Collections.Generic;

namespace VMindbooke.Bot.Domain
{
    public class User
    {
        public int Id { get; }
        
        public string Token { get; }
        
        public string Name { get; }
        
        public IEnumerable<Like> Likes { get; }
    }
}