namespace VMindbooke.Bot.Domain
{
    public interface IShitPostFactory
    {
        string GenerateComment();

        string GenerateReply();
        string GenerateTitle();
    }
}