using System;

namespace VMindbookeSDK.Entities
{
    public class NewComment
    {
        public string Content { get; }

        public NewComment(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
