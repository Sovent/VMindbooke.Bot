using VMindbooke.SDK;

namespace LikesCheating.Domain
{
    public class Cheating : ICheating
    {
        public void CommentIfLikesMoreThanThreshold(Post post, int likesThreshold, string token)
        {
            
        }

        public void ReplyIfLikesMoreThanThreshold(Comment comment, int likesThreshold, string token)
        {
            
        }

        public void DuplicatePostIfLikesMoreThanThreshold(Post post, int likesThreshold, string token)
        {
            
        }

        public void DuplicateMostSuccessfulPostIfLikesMoreThanThreshold(
            Post post, User successUser, int likesThreshold, string token)
        {
            
        }
    }
}