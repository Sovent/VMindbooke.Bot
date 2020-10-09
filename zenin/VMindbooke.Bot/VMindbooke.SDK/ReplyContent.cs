namespace VMindbooke.SDK
{
    public class ReplyContent : IContent
    {
        public string Content { get; }

        public ReplyContent(string content)
        {
            Content = content;
        }
    }
}