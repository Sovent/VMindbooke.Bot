namespace VMindbookeBooster.Entities
{
    public class PostContent
    {
        public PostContent(string title, string content)
        {
            Title = title;
            Content = content;
        }
        
        public string Title { get; }
        
        public string Content { get; }
    }
}