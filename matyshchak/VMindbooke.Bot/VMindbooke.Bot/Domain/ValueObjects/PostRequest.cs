namespace Usage.Domain.ContentProviders
{
    public class PostRequest
    {
        public PostRequest(string title, string content)
        {
            Title = title;
            Content = content;
        }
        
        public string Title { get; }
        
        public string Content { get; }
    }
}