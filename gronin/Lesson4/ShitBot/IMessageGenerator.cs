namespace ShitBot
{
    public  interface IMessageGenerator
    {
        string GetComment();
        string GetTitle();
        string GetReply();
    }
}