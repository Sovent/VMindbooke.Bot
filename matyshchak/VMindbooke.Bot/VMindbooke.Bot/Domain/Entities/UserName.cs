namespace Usage.Domain.Entities
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