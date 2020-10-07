using VMindbooke.SDK;

namespace LikesCheating.Domain
{
    public interface ICheating
    {
        void CommentIfLikesMoreThanThreshold(Post post, int likesThreshold, string token);
        void ReplyIfLikesMoreThanThreshold(Comment comment, int likesThreshold, string token);
        void DuplicatePostIfLikesMoreThanThreshold(Post post, int likesThreshold, string token);
        void DuplicateMostSuccessfulPostIfLikesMoreThanThreshold(
            Post post, User checkingUser, int likesThreshold, string token);
    }
}