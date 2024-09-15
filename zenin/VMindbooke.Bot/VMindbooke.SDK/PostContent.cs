namespace VMindbooke.SDK
{
    public class PostContent : IContent
    {
        public string Title { get; }
        public string Content { get; }

        public PostContent(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}