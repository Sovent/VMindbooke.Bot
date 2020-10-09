using System;

namespace VMindbooke.SDK.Model
{
    public class NewComment
    {
        public NewComment(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public string Content { get; }
    }
}
