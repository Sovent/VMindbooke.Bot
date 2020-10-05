using System;

namespace VMindbooke.SDK.Model
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
