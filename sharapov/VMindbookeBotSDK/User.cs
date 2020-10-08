using System.Collections.ObjectModel;

namespace VMindbookeBot
{
    public class User
    {
        public int id { get; }
        public string token { get; }
        public string name { get; }
        public ReadOnlyCollection<Like> likes { get; }
        
        public User(ReadOnlyCollection<Like> likes, int id, string token, string name)
        {
            this.id = id;
            this.token = token;
            this.likes = likes;
            this.name = name;
        }
    }
}