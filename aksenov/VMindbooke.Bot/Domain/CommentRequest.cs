namespace VMindbooke.Bot.Domain
{
    public class CommentRequest
    {
        public CommentRequest(string content)
        {
            Content = content;
        }

        public string Content { get; }
    }
}