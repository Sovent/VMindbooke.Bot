namespace Usage.Domain.Entities
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