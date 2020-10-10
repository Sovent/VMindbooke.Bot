using VMindbookeBooster.Entities;

namespace VMindbookeBooster
{
    public interface IPostCommenter
    {
        void CommentPosts(int likesThreshold, CommentContent comment);
    }
}