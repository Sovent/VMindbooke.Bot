using System;

namespace VMindbookeSDK.Entities
{
    public class NewUser
    {
        public string Name { get; }

        public NewUser(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
