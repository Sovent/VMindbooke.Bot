namespace Usage.Domain.ContentProviders
{
    public class CommentContent
    {
        public CommentContent(string content)
        {
            Content = content;
        }

        public string Content { get; }
    }
}