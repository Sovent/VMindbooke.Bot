using System;

namespace VMindbooster
{
    public interface IUsedContentRegistry
    {
        bool IsCommentedPost(int postId);

        bool IsRepliedComment(Guid commentId);

        void MarkPostCommented(int postId);

        void MarkCommentReplied(Guid commentId);
    }
}