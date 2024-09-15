using VMindbooke.SDK;
using System.Collections.Generic;

namespace LikesCheating.Domain
{
    public interface ICheating
    {
        void CommentIfLikesMoreThanThreshold(string token);
        void ReplyIfLikesMoreThanThreshold(string token);
        void DuplicatePostIfLikesMoreThanThreshold(int userId, string token);
        void DuplicateMostSuccessfulPostIfLikesMoreThanThreshold(int userId, string token);
        bool CheckUserReachedLikesThreshold(int userId);
    }
}