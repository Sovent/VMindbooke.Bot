using System;

namespace VMindbooke.SDK.Model
{
    public class NewPost
    {
        public NewPost(string title, string content)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public string Title { get; }
        public string Content { get; }
    }
}
