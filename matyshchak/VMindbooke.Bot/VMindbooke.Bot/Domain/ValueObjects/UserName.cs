﻿namespace Usage.Domain.ValueObjects
{
    public class UserName
    {
        public UserName(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}