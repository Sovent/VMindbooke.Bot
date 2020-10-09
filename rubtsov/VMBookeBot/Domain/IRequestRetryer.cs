using System;

namespace VMBookeBot.Domain
{
    public interface IRequestRetryer
    {
        public bool TryWriteComment(int postId, int retryCount = 5);
        public bool TryWriteReplyUnderComment(int postId, Guid commentId, int retryCount = 5);
        public bool TryMakeCopyOfPost(string postTitle, string postContent, int originalPostId, int retryCount = 5);
    }
}