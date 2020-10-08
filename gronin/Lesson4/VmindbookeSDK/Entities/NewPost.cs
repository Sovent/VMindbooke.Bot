using System;

namespace VMindbookeSDK.Entities
{
    public class NewPost
    {
        public string Title { get; }
        public string Content { get; }

        public NewPost(string title, string content)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
