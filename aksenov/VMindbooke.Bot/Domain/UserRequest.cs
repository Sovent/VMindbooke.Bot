namespace VMindbooke.Bot.Domain
{
    public class UserRequest
    {
        public UserRequest(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}