namespace Usage.Domain.ValueObjects
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