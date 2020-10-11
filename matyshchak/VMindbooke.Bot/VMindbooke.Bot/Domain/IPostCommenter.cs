using Usage.Domain.Entities;

namespace Usage.Domain
{
    public interface IPostCommenter
    {
        void CommentPosts(int likesThreshold);
    }
}