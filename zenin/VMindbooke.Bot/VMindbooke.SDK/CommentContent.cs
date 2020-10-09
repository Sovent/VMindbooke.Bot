namespace VMindbooke.SDK
{
    public class CommentContent : IContent
    {
        public string Content { get; }

        public CommentContent(string content)
        {
            Content = content;
        }
        
    }
}