using VMindbooke.Bot.Domain;

namespace VMindbooke.Bot.Infrastructure
{
    public class NaiveShitPostFactory : IShitPostFactory
    {
        public string GenerateComment()
        {
            return "I am test comment!!!";
        }

        public string GenerateReply()
        {
            return "I am test reply!!!";
        }

        public string GenerateTitle()
        {
            return "I am test header!!!";
        }
    }
}