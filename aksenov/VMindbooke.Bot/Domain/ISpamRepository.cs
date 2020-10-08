namespace VMindbooke.Bot.Domain
{
    public interface ISpamRepository
    {
        string GetRandomPostTitle();
        string GetRandomComment();
        string GetRandomReply();
    }
}