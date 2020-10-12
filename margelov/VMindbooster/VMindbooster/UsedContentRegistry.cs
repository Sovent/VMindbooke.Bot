using System;

namespace VMindbooster
{
    public class UsedContentRegistry : IUsedContentRegistry
    {
        public bool IsCommentedPost(int postId)
        {
            throw new NotImplementedException();
        }

        public bool IsRepliedComment(Guid commentId)
        {
            throw new NotImplementedException();
        }

        public void MarkPostCommented(int postId)
        {
            throw new NotImplementedException();
        }

        public void MarkCommentReplied(Guid commentId)
        {
            throw new NotImplementedException();
        }
    }
}