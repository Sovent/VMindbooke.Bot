﻿namespace Usage.Domain.ValueObjects
{
    public class UserCredentials
    {
        public UserCredentials(int id, string token)
        {
            Id = id;
            Token = token;
        }
        
        public int Id { get; }
        public string Token { get; }
    }
}