using System;

namespace VMindbooke.SDK.Model
{
    public class NewUser
    {
        public NewUser(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}
